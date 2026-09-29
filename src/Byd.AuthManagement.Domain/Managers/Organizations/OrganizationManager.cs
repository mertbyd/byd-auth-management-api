using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Models.Organizations;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

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
    IIdentityUserRepository userRepository,
    IDataFilter dataFilter) : DomainService
{
    // Kullanicinin istegin tenant'indaki uyeliklerini getirir; uyelik kaydi kullanicinin kendi tenant'inda durabilir.
    public virtual async Task<List<OrganizationModel>> GetUserOrganizationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var organizations = await repository.GetUserModelListAsync(userId, cancellationToken);
        return organizations.FindAll(organization => organization.TenantId == CurrentTenant.Id);
    }

    // Create modelinden istegin tenant'inda yeni organizasyon kurup agaca ekler.
    public virtual async Task<OrganizationUnit> CreateAsync(
        OrganizationCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = new OrganizationUnit(GuidGenerator.Create(), model.DisplayName, model.ParentId, CurrentTenant.Id);
        await organizationUnitManager.CreateAsync(entity);
        return entity;
    }

    // Ust kayit degistiyse kaydi alt agaciyla tasir, sonra adini gunceller.
    public virtual async Task UpdateAsync(
        Guid id,
        OrganizationUpdateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(id, includeDetails: false, cancellationToken);
        if (entity.ParentId != model.ParentId)
            await organizationUnitManager.MoveAsync(id, model.ParentId);

        entity.DisplayName = model.DisplayName;
        await organizationUnitManager.UpdateAsync(entity);
    }

    // Kullaniciyi organizasyona uye yapar. Organizasyon tenant filtresi acikken okunur ki baska tenant'in
    // organizasyonuna uye eklenemesin; kullanici ve uyelikleri host'ta durdugu icin gerisi filtre kapaliyken yapilir.
    public virtual async Task AddMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var organization = await repository.GetAsync(id, includeDetails: false, cancellationToken);
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var user = await userRepository.GetAsync(userId, includeDetails: false, cancellationToken);
        await userManager.AddToOrganizationUnitAsync(user, organization);
    }

    // Kullaniciyi organizasyondan cikarir; okuma kurali uye eklemeyle aynidir.
    public virtual async Task RemoveMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var organization = await repository.GetAsync(id, includeDetails: false, cancellationToken);
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var user = await userRepository.GetAsync(userId, includeDetails: false, cancellationToken);
        await userManager.RemoveFromOrganizationUnitAsync(user, organization);
    }
}
