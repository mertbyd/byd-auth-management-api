using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Extensions.Organizations;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Managers.Accounts;
using Byd.AuthManagement.Managers.Features;
using Byd.AuthManagement.Models.Organizations;
using Byd.AuthManagement.Permissions;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Byd.AuthManagement.Managers.Organizations;

/// <summary>
/// Organizasyonun olusturma, guncelleme ve uyelik kurallarini yonetir.
/// </summary>
// islevi: Organizasyonu istegin tenant'inda kurar ve gunceller; uyelikleri okur, uye ekler ve cikarir.
// sistemdeki gorevi: Agac kurallarini (kod, tekillik, tasima) cercevenin Manager'ina birakan domain sahibidir.
public class OrganizationManager(
    IOrganizationRepository repository,
    OrganizationUnitManager organizationUnitManager,
    IdentityUserManager userManager,
    IdentityRoleManager roleManager,
    IIdentityRoleRepository roleRepository,
    IIdentityUserRepository userRepository,
    IPermissionDataSeeder permissionDataSeeder,
    IPermissionManager permissionManager,
    IFeatureManager featureManager,
    FeatureSelectionManager featureSelectionManager,
    AccountManager accountManager,
    ICurrentOrganization currentOrganization,
    IDataFilter dataFilter) : DomainService
{
    private static readonly string[] AdminFeaturePermissions =
    [
        AuthManagementPermissions.Features.Default,
        AuthManagementPermissions.Features.Manage
    ];

    // Organizasyonu, temel admin rolunu ve yonetici hesabini ayni kullanim senaryosunda kurar.
    public virtual async Task<CreatedOrganizationModel> CreateAsync(
        OrganizationCreateModel model,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanCreateAsync(model.ParentId, cancellationToken);
        await featureSelectionManager.ValidateAsync(model.Features, cancellationToken);
        var parent = model.ParentId is null
            ? null
            : await repository.GetAsync(model.ParentId.Value, false, cancellationToken);
        var entity = new OrganizationUnit(GuidGenerator.Create(), model.DisplayName, model.ParentId, CurrentTenant.Id);
        await organizationUnitManager.CreateAsync(entity);
        var role = await CreateAdminRoleAsync(entity, cancellationToken);
        var (user, isNewUser) = await accountManager.FindOrCreateHostUserAsync(
            model.AdminUserName,
            model.AdminEmail,
            model.AdminPassword,
            cancellationToken);
        await AssignAdminAsync(entity, role, user, cancellationToken);
        await featureSelectionManager.ReplaceAsync(
            FeatureConsts.OrganizationProviderName, entity.Id.ToString(), model.Features, cancellationToken);
        return entity.ToCreatedOrganizationModel(parent?.DisplayName, role, user, isNewUser, model.Features);
    }

    // Feature yonetimini istegin tenant'i ve mevcut organizasyonun alt agaciyla sinirlar.
    public virtual async Task EnsureCanManageFeaturesAsync(
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(providerKey, out var organizationId))
            throw new BusinessException(FeatureExceptionCodes.OutsideScope);
        var target = await repository.FindAsync(organizationId, false, cancellationToken);
        if (target is null || target.TenantId != CurrentTenant.Id)
            throw new BusinessException(FeatureExceptionCodes.OutsideScope);
        if (!currentOrganization.IsAvailable)
            return;
        var current = await GetCurrentOrganizationAsync(cancellationToken);
        if (!target.IsWithinScopeOf(current))
            throw new BusinessException(FeatureExceptionCodes.OutsideScope);
    }

    // Ust kayit degistiyse kaydi alt agaciyla tasir, sonra adini gunceller.
    public virtual async Task UpdateAsync(
        Guid id,
        OrganizationUpdateModel model,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanUpdateAsync(id, model.ParentId, cancellationToken);
        await featureSelectionManager.ValidateAsync(model.Features, cancellationToken);
        var entity = await repository.GetAsync(id, includeDetails: false, cancellationToken);
        if (entity.ParentId != model.ParentId)
            await organizationUnitManager.MoveAsync(id, model.ParentId);

        entity.DisplayName = model.DisplayName;
        await organizationUnitManager.UpdateAsync(entity);
        await featureSelectionManager.ReplaceAsync(
            FeatureConsts.OrganizationProviderName, id.ToString(), model.Features, cancellationToken);
    }

    // Kullaniciyi organizasyona uye yapar. Organizasyon tenant filtresi acikken okunur ki baska tenant'in
    // organizasyonuna uye eklenemesin; kullanici ve uyelikleri host'ta durdugu icin gerisi filtre kapaliyken yapilir.
    public virtual async Task AddMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var (organization, user) = await GetMembershipEntitiesAsync(id, userId, cancellationToken);
        await userManager.AddToOrganizationAsync(CurrentTenant, user, organization, cancellationToken);
    }

    // Kullaniciyi organizasyondan cikarir; okuma kurali uye eklemeyle aynidir.
    public virtual async Task RemoveMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var (organization, user) = await GetMembershipEntitiesAsync(id, userId, cancellationToken);
        await userManager.RemoveFromOrganizationAsync(CurrentTenant, user, organization, cancellationToken);
    }

    // Alt agactaki organizasyon rollerinin izinlerini ve rolleri temizleyip organizasyon agacini siler.
    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await EnsureCanDeleteAsync(id, cancellationToken);
        var root = await repository.GetAsync(id, true, cancellationToken);
        var organizations = await organizationUnitManager.FindChildrenAsync(id, recursive: true);
        organizations.Add(root);
        var roleIds = organizations.SelectMany(organization => organization.Roles)
            .Select(binding => binding.RoleId)
            .Distinct()
            .ToList();
        var roles = await roleRepository.GetListAsync(roleIds, cancellationToken);
        foreach (var role in roles)
            await DeleteOrganizationRoleAsync(role, cancellationToken);
        foreach (var organization in organizations)
            await featureManager.DeleteAsync(FeatureConsts.OrganizationProviderName, organization.Id.ToString());
        await organizationUnitManager.DeleteAsync(id);
    }

    // Organizasyona benzersiz, statik ve public temel admin rolunu baglayip gerekli izinleri tohumlar.
    private async Task<IdentityRole> CreateAdminRoleAsync(
        OrganizationUnit organization,
        CancellationToken cancellationToken = default)
    {
        var roleName = OrganizationConsts.AdminRoleNamePrefix + organization.Id.ToString("N");
        var role = new IdentityRole(GuidGenerator.Create(), roleName, organization.TenantId)
        {
            IsStatic = true,
            IsPublic = true,
            IsDefault = false
        };
        (await roleManager.CreateAsync(role)).CheckErrors();
        await organizationUnitManager.AddRoleToOrganizationUnitAsync(role, organization);
        await permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            roleName,
            GetAdminPermissionNames(),
            organization.TenantId);
        return role;
    }

    // Yoneticiye organizasyon rolunu dogrudan verir ve organizasyonu secilebilir yapan uyeligi acar.
    private async Task AssignAdminAsync(
        OrganizationUnit organization,
        IdentityRole role,
        IdentityUser user,
        CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(organization.TenantId))
            (await userManager.AddToRoleAsync(user, role.Name)).CheckErrors();

        await userManager.AddToOrganizationAsync(CurrentTenant, user, organization, cancellationToken);
    }

    // Organizasyon admin rolunun alacagi sabit izin kumesini dondurur.
    private static string[] GetAdminPermissionNames()
        =>
        [
            AuthManagementPermissions.Users.Default,
            AuthManagementPermissions.Users.Manage,
            AuthManagementPermissions.Organizations.Default,
            AuthManagementPermissions.Organizations.View,
            AuthManagementPermissions.Organizations.Manage,
            ..AdminFeaturePermissions
        ];

    // Izinleri acikca temizler, statik korumayi kontrollu kaldirir ve rol silmeyle kullanici baglarini cascade eder.
    private async Task DeleteOrganizationRoleAsync(
        IdentityRole role,
        CancellationToken cancellationToken = default)
    {
        await permissionManager.DeleteAsync(RolePermissionValueProvider.ProviderName, role.Name);
        role.IsStatic = false;
        (await roleManager.DeleteAsync(role)).CheckErrors();
    }

    // Uye ekleme ve cikarma icin ayni kapsam, organizasyon ve kullanici yukleme adimlarini tek yerde isletir.
    private async Task<(OrganizationUnit Organization, IdentityUser User)> GetMembershipEntitiesAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanManageMemberAsync(organizationId, cancellationToken);
        var organization = await repository.GetAsync(organizationId, includeDetails: false, cancellationToken);
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var user = await userRepository.GetAsync(userId, includeDetails: false, cancellationToken);
        return (organization, user);
    }

    // Yeni organizasyonun ustunu mevcut organizasyon veya alt agaciyla sinirlar.
    private async Task EnsureCanCreateAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        if (!currentOrganization.IsAvailable)
            return;
        if (parentId is null)
            throw new BusinessException(OrganizationExceptionCodes.OutsideScope);
        await EnsureWithinCurrentAsync(parentId.Value, includeCurrent: true, cancellationToken);
    }

    // Guncellenecek organizasyonu ve yeni ustunu mevcut organizasyon veya alt agaciyla sinirlar.
    private async Task EnsureCanUpdateAsync(
        Guid id,
        Guid? parentId,
        CancellationToken cancellationToken = default)
    {
        if (!currentOrganization.IsAvailable)
            return;
        if (parentId is null)
            throw new BusinessException(OrganizationExceptionCodes.OutsideScope);
        var current = await GetCurrentOrganizationAsync(cancellationToken);
        await EnsureWithinCurrentAsync(current, id, includeCurrent: true, cancellationToken);
        await EnsureWithinCurrentAsync(current, parentId.Value, includeCurrent: true, cancellationToken);
    }

    // Silmeyi mevcut organizasyonun yalniz alt kayitlariyla sinirlar.
    private async Task EnsureCanDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!currentOrganization.IsAvailable)
            return;
        await EnsureWithinCurrentAsync(id, includeCurrent: false, cancellationToken);
    }

    // Uyelik islemini mevcut organizasyon veya alt agaciyla sinirlar.
    private async Task EnsureCanManageMemberAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!currentOrganization.IsAvailable)
            return;
        await EnsureWithinCurrentAsync(id, includeCurrent: true, cancellationToken);
    }

    // Hedef organizasyonun mevcut organizasyonun izin verilen alt agacinda oldugunu kod uzerinden denetler.
    private async Task EnsureWithinCurrentAsync(
        Guid targetId,
        bool includeCurrent,
        CancellationToken cancellationToken = default)
    {
        var current = await GetCurrentOrganizationAsync(cancellationToken);
        await EnsureWithinCurrentAsync(current, targetId, includeCurrent, cancellationToken);
    }

    // Mevcut organizasyon kapsam kaydini tek repository cagrisiyla getirir.
    private async Task<OrganizationUnit> GetCurrentOrganizationAsync(
        CancellationToken cancellationToken = default)
        => await repository.GetAsync(currentOrganization.Id!.Value, false, cancellationToken);

    // Yuklenmis mevcut organizasyona gore hedefin alt agac sinirini denetler.
    private async Task EnsureWithinCurrentAsync(
        OrganizationUnit current,
        Guid targetId,
        bool includeCurrent,
        CancellationToken cancellationToken = default)
    {
        var target = await repository.GetAsync(targetId, false, cancellationToken);
        if (!target.IsWithinScopeOf(current, includeCurrent))
            throw new BusinessException(OrganizationExceptionCodes.OutsideScope);
    }
}
