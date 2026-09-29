using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Managers.Organizations;
using Byd.AuthManagement.Models.Logins;
using Byd.AuthManagement.Models.Registrations;
using Byd.CoreExtensions.Consts;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using IdentityOptions = Microsoft.AspNetCore.Identity.IdentityOptions;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Byd.AuthManagement.Managers.Accounts;

/// <summary>
/// Hesabin girisini, token kapsamini ve kaydini yonetir.
/// </summary>
// islevi: Kimlik bilgisini dogrular, kapsamlari rollerden ve uyeliklerden cikarir, secimi dogrulayip rolleri toplar; kullaniciyi kaydeder.
// sistemdeki gorevi: Hesap servisi ve token claim'lerini kuran handler kapsam ve kayit kararlarini buradan alir.
// Kullanici host'ta, rolleri ve organizasyonlari tenant'larda durdugu icin okumalar tenant filtresi kapaliyken yapilir.
public class AccountManager(
    IdentityUserManager userManager,
    IIdentityUserRepository userRepository,
    IIdentityRoleRepository roleRepository,
    IOrganizationRepository organizationRepository,
    OrganizationManager organizationManager,
    ITenantStore tenantStore,
    IOptions<IdentityOptions> identityOptions,
    IDataFilter dataFilter,
    ICurrentUser currentUser,
    ICurrentOrganization currentOrganization) : DomainService
{
    // Kullaniciyi adina ya da e-postasina gore bulur; bulunamazsa gecersiz kimlik bilgisi hatasi verir.
    public virtual async Task<IdentityUser> GetUserAsync(
        string userNameOrEmail,
        CancellationToken cancellationToken = default)
        => await userManager.FindByNameAsync(userNameOrEmail)
           ?? await userManager.FindByEmailAsync(userNameOrEmail)
           ?? throw new BusinessException(LoginExceptionCodes.InvalidCredentials);

    // Sifre kontrolunun sonucunu hata koduna cevirir.
    public virtual void EnsureSignedIn(SignInResult result)
    {
        if (result.IsLockedOut)
            throw new BusinessException(LoginExceptionCodes.LockedOut);

        if (result.IsNotAllowed)
            throw new BusinessException(LoginExceptionCodes.NotAllowed);

        if (!result.Succeeded)
            throw new BusinessException(LoginExceptionCodes.InvalidCredentials);
    }

    // Kullanicinin girebilecegi tenant'lari ve uyesi oldugu organizasyonlari getirir.
    public virtual async Task<LoginScopeModel> GetScopeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var roles = await GetDirectRolesAsync(userId, cancellationToken);
        var organizations = await organizationRepository.GetUserModelListAsync(userId, cancellationToken);
        var tenantIds = GetTenantIds(roles, organizations.Select(organization => organization.TenantId));
        return new LoginScopeModel
        {
            Tenants = BuildTenants(tenantIds, roles, await tenantStore.GetListAsync()),
            Organizations = organizations
        };
    }

    // Token'a yazilacak kapsami belirler; secim eksikse kapsamsiz ve rolsuz secim doner.
    public virtual async Task<LoginSelectionModel> ResolveTokenSelectionAsync(
        Guid userId,
        LoginScopeModel scope,
        Guid? tenantId,
        Guid? organizationId,
        CancellationToken cancellationToken = default)
        => RequiresSelection(scope, tenantId, organizationId)
            ? new LoginSelectionModel { IsSelectionRequired = true }
            : await ResolveSelectionAsync(userId, tenantId, organizationId, cancellationToken);

    // Secimi dogrular, secilmeyen kapsami tek secenekse tamamlar ve kapsamdaki rolleri toplar.
    public virtual async Task<LoginSelectionModel> ResolveSelectionAsync(
        Guid userId,
        Guid? tenantId,
        Guid? organizationId,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var roles = await GetDirectRolesAsync(userId, cancellationToken);
        var organizations = await userRepository.GetOrganizationUnitsAsync(
            userId,
            includeDetails: true,
            cancellationToken);
        var tenantIds = GetTenantIds(roles, organizations.Select(organization => organization.TenantId));
        var selectedTenantId = ResolveTenant(tenantIds, tenantId);
        var organization = ResolveOrganization(organizations, selectedTenantId, organizationId);
        var scopeRoles = await GetScopeRolesAsync(roles, selectedTenantId, organization, cancellationToken);
        if (scopeRoles.Count == 0)
            throw new BusinessException(LoginExceptionCodes.NoRoleInScope);

        return new LoginSelectionModel
        {
            TenantId = selectedTenantId,
            TenantName = selectedTenantId is null ? null : (await tenantStore.FindAsync(selectedTenantId.Value))?.Name,
            OrganizationId = organization?.Id,
            OrganizationName = organization?.DisplayName,
            Roles = scopeRoles
        };
    }

    // Istegi yapan kullaniciyi token'daki kapsam ve rollerle, oturumdaki tenant'taki uyelikleriyle getirir.
    public virtual async Task<CurrentLoginModel> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUser.GetId();
        var roleIds = currentUser.FindClaims(RoleClaimTypes.RoleId).Select(claim => Guid.Parse(claim.Value));
        return new CurrentLoginModel
        {
            TenantId = CurrentTenant.Id,
            TenantName = CurrentTenant.Name,
            OrganizationId = currentOrganization.Id,
            OrganizationName = currentOrganization.Name,
            UserId = userId,
            UserName = currentUser.UserName!,
            Name = currentUser.Name,
            Surname = currentUser.SurName,
            Email = currentUser.Email,
            Roles = await roleRepository.GetListAsync(roleIds, cancellationToken),
            Organizations = await organizationManager.GetUserOrganizationsAsync(userId, cancellationToken)
        };
    }

    // Kullaniciyi host'ta acar ve secilen tenant'in varsayilan rollerini verir; organizasyon uyeligi yonetici ucunda verilir.
    public virtual async Task<RegisteredUserModel> RegisterAsync(
        RegistrationModel model,
        CancellationToken cancellationToken = default)
    {
        var tenant = await GetActiveTenantAsync(model.TenantId, cancellationToken);
        await EnsureDefaultRoleExistsAsync(tenant.Id, cancellationToken);
        var user = await CreateHostUserAsync(model, cancellationToken);
        using (CurrentTenant.Change(tenant.Id))
            (await userManager.AddDefaultRolesAsync(user)).CheckErrors();

        return new RegisteredUserModel
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    // Kullanicinin rol baglarindan dogrudan rollerini okur.
    private async Task<List<IdentityRole>> GetDirectRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(userId, includeDetails: true, cancellationToken);
        return await roleRepository.GetListAsync(user.Roles.Select(role => role.RoleId), cancellationToken);
    }

    // Tenant'lari rollerden ve uyeliklerden toplar; hicbiri yoksa ve host rolu varsa host'u (bos kimlik) verir.
    private static List<Guid?> GetTenantIds(List<IdentityRole> roles, IEnumerable<Guid?> organizationTenantIds)
    {
        List<Guid?> tenantIds =
        [
            .. roles.Select(role => role.TenantId)
                .Concat(organizationTenantIds)
                .Where(id => id.HasValue)
                .Distinct()
        ];
        if (tenantIds.Count == 0 && roles.Exists(role => role.TenantId is null))
            tenantIds.Add(null);

        return tenantIds;
    }

    // Tenant satirlarini adlariyla ve kullanicinin orada dogrudan rolu olup olmadigiyla kurar.
    private static List<LoginTenantOptionModel> BuildTenants(
        List<Guid?> tenantIds,
        List<IdentityRole> roles,
        IReadOnlyList<TenantConfiguration> tenants)
        => tenantIds.ConvertAll(id => new LoginTenantOptionModel
        {
            Id = id,
            Name = tenants.FirstOrDefault(tenant => tenant.Id == id)?.Name,
            HasRole = roles.Exists(role => role.TenantId == id)
        });

    // Tenant ya da tenant'taki organizasyon birden fazla secenekken secilmediyse secim eksiktir.
    private static bool RequiresSelection(LoginScopeModel scope, Guid? tenantId, Guid? organizationId)
    {
        if (tenantId is null && scope.Tenants.Count > 1)
            return true;

        var tenant = tenantId ?? scope.Tenants.SingleOrDefault()?.Id;
        return organizationId is null && scope.Organizations.Count(organization => organization.TenantId == tenant) > 1;
    }

    // Secilen tenant kullanicinin tenant'larindan biri olmali; secilmediyse tek secenek alinir.
    private static Guid? ResolveTenant(List<Guid?> tenantIds, Guid? tenantId)
    {
        if (tenantId is null && tenantIds.Count <= 1)
            return tenantIds.SingleOrDefault();

        if (!tenantIds.Contains(tenantId))
            throw new BusinessException(LoginExceptionCodes.ScopeNotAvailable);

        return tenantId;
    }

    // Secilen organizasyon tenant'taki uyeliklerden biri olmali; secilmediyse tek uyelik alinir.
    private static OrganizationUnit? ResolveOrganization(
        List<OrganizationUnit> organizations,
        Guid? tenantId,
        Guid? organizationId)
    {
        var candidates = organizations.FindAll(organization => organization.TenantId == tenantId);
        if (organizationId is null && candidates.Count <= 1)
            return candidates.SingleOrDefault();

        return candidates.Find(organization => organization.Id == organizationId)
               ?? throw new BusinessException(LoginExceptionCodes.ScopeNotAvailable);
    }

    // Tenant'taki dogrudan rollere secilen organizasyonun rollerini ekler.
    private async Task<List<IdentityRole>> GetScopeRolesAsync(
        List<IdentityRole> directRoles,
        Guid? tenantId,
        OrganizationUnit? organization,
        CancellationToken cancellationToken = default)
    {
        List<IdentityRole> organizationRoles = organization is null
            ? []
            : await roleRepository.GetListAsync(organization.Roles.Select(role => role.RoleId), cancellationToken);
        return [.. directRoles.Where(role => role.TenantId == tenantId).UnionBy(organizationRoles, role => role.Id)];
    }

    // Secilen tenant var ve aktif olmali.
    private async Task<TenantConfiguration> GetActiveTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await tenantStore.FindAsync(tenantId);
        if (tenant is null || !tenant.IsActive)
            throw new BusinessException(RegistrationExceptionCodes.TenantNotAvailable);

        return tenant;
    }

    // Tenant'ta varsayilan rol yoksa tenant kayda kapalidir; rolsuz kullanici login olamazdi.
    private async Task EnsureDefaultRoleExistsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var defaultRoles = await roleRepository.GetDefaultOnesAsync(includeDetails: false, cancellationToken);
            if (defaultRoles.Count == 0)
                throw new BusinessException(RegistrationExceptionCodes.NoDefaultRole);
        }
    }

    // Kullaniciyi host'ta, cercevenin sifre ve tekillik kurallariyla acar.
    private async Task<IdentityUser> CreateHostUserAsync(
        RegistrationModel model,
        CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
        {
            await identityOptions.SetAsync();
            var user = new IdentityUser(GuidGenerator.Create(), model.UserName, model.Email);
            (await userManager.CreateAsync(user, model.Password)).CheckErrors();
            return user;
        }
    }
}
