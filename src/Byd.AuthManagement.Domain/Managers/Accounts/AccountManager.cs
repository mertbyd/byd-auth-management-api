using System.Text.Encodings.Web;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Extensions.Accounts;
using Byd.AuthManagement.Extensions.Organizations;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Models.Invitations;
using Byd.AuthManagement.Models.Logins;
using Byd.AuthManagement.Models.Organizations;
using Byd.AuthManagement.Models.Registrations;
using Byd.CoreExtensions.Consts;
using Byd.CoreExtensions.Interfaces;
using Byd.CoreExtensions.Scopes;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;
using IdentityOptions = Microsoft.AspNetCore.Identity.IdentityOptions;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Byd.AuthManagement.Managers.Accounts;

/// <summary>
/// Hesabin girisini, token kapsamini ve kaydini yonetir.
/// </summary>
// islevi: Kimlik bilgisini dogrular, kapsamlari rollerden ve uyeliklerden cikarir, secimi dogrulayip rolleri toplar;
// kullaniciyi kaydeder ya da rolleriyle davet eder.
// sistemdeki gorevi: Hesap servisi ve token claim'lerini kuran handler kapsam, kayit ve davet kararlarini buradan alir.
// Kullanici host'ta, rolleri ve organizasyonlari tenant'larda durdugu icin okumalar tenant filtresi kapaliyken yapilir.
public class AccountManager(
    IdentityUserManager userManager,
    IdentityRoleManager roleManager,
    IIdentityUserRepository userRepository,
    IIdentityRoleRepository roleRepository,
    IOrganizationRepository organizationRepository,
    ITenantManager tenantManager,
    ITenantRepository tenantRepository,
    ITenantStore tenantStore,
    IPermissionDefinitionManager permissionDefinitionManager,
    IPermissionDataSeeder permissionDataSeeder,
    IOptions<IdentityOptions> identityOptions,
    IDataFilter dataFilter,
    ICurrentUser currentUser,
    ICurrentOrganization currentOrganization) : DomainService
{
    // Kullaniciyi istekteki tenant'tan bagimsiz, adina ya da e-postasina gore bulur; bulunamazsa gecersiz kimlik
    // bilgisi hatasi verir.
    public virtual async Task<IdentityUser> GetUserAsync(
        string userNameOrEmail,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        return await userManager.FindByNameAsync(userNameOrEmail)
               ?? await userManager.FindByEmailAsync(userNameOrEmail)
               ?? throw new BusinessException(LoginExceptionCodes.InvalidCredentials);
    }

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
        var roleOrganizations = await GetRoleOrganizationsAsync(
            roles.Select(role => role.Id),
            cancellationToken);
        var tenantIds = GetTenantIds(roles, organizations.Select(organization => organization.TenantId));
        return new LoginScopeModel
        {
            Tenants = BuildTenants(tenantIds, roles, roleOrganizations.Keys, await tenantStore.GetListAsync()),
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
            : await ResolveSelectionAsync(userId, tenantId, organizationId, selectSingleScope: true, cancellationToken);

    // Acik secimi dogrular; yalniz ilk login akisinda tek secenegi otomatik tamamlar.
    public virtual async Task<LoginSelectionModel> ResolveSelectionAsync(
        Guid userId,
        Guid? tenantId,
        Guid? organizationId,
        bool selectSingleScope = false,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var roles = await GetDirectRolesAsync(userId, cancellationToken);
        var organizations = await userRepository.GetOrganizationUnitsAsync(
            userId,
            includeDetails: true,
            cancellationToken);
        var tenantIds = GetTenantIds(roles, organizations.Select(organization => organization.TenantId));
        var selectedTenantId = ResolveTenant(tenantIds, tenantId, selectSingleScope);
        var organization = ResolveOrganization(organizations, selectedTenantId, organizationId, selectSingleScope);
        var scopeRoles = await GetScopeRolesAsync(roles, selectedTenantId, organization, cancellationToken);
        if (scopeRoles.Count == 0)
            throw new BusinessException(LoginExceptionCodes.NoRoleInScope);
        var parent = await FindParentOrganizationAsync(organization, cancellationToken);
        return new LoginSelectionModel
        {
            TenantId = selectedTenantId,
            TenantName = selectedTenantId is null ? null : (await tenantStore.FindAsync(selectedTenantId.Value))?.Name,
            OrganizationId = organization?.Id,
            OrganizationName = organization?.DisplayName,
            ParentOrganizationId = parent?.Id,
            ParentOrganizationName = parent?.DisplayName,
            ScopeLevel = ResolveScopeLevel(selectedTenantId, organization),
            Roles = scopeRoles
        };
    }
    // Istegi yapan kullaniciyi token'daki kapsam ve rollerle, oturumdaki tenant'taki uyelikleriyle getirir.
    public virtual async Task<CurrentLoginModel> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUser.GetId();
        var roleIds = currentUser.FindClaims(RoleClaimTypes.RoleId).Select(claim => Guid.Parse(claim.Value));
        var roles = await roleRepository.GetListAsync(roleIds, cancellationToken);
        return currentUser.ToCurrentLoginModel(
            userId,
            CurrentTenant.Id,
            CurrentTenant.Name,
            currentOrganization.Id,
            currentOrganization.Name,
            Enum.TryParse<ScopeLevel>(currentUser.FindClaim(ScopeClaimTypes.Level)?.Value, out var level)
                ? level : ScopeLevel.Unselected,
            await BuildRoleModelsAsync(roles, cancellationToken),
            await GetUserOrganizationsAsync(userId, cancellationToken));
    }

    // Organizasyon kurulurken e-posta kayitliysa host hesabini korur; degilse sifreli yeni hesap acar.
    public virtual async Task<(IdentityUser User, bool IsNewUser)> FindOrCreateHostUserAsync(
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await FindHostUserByEmailAsync(email);
        if (user is not null)
            return (user, false);

        return (await CreateHostUserAsync(userName, email, password, cancellationToken), true);
    }
    // Kullaniciyi host'ta acar. Sistemdeki aktif tenant'i alir ve tenant'in rollerini UserRoles tablosunda esler.
    public virtual async Task<RegisteredUserModel> RegisterAsync(
        RegistrationModel model,
        CancellationToken cancellationToken = default)
    {
        var tenantId = await ResolveRegistrationTenantIdAsync(cancellationToken);
        var scope = await GetTenantScopeAsync(tenantId, cancellationToken);
        return await RegisterUserAsync(scope, model.UserName, model.Email, model.Password, cancellationToken);
    }

    // Kaydolunacak tenant'i belirler; sirasiyla oturum tenant'i veya sistemdeki ilk aktif tenant'i secer.
    private async Task<Guid> ResolveRegistrationTenantIdAsync(
        CancellationToken cancellationToken = default)
    {
        if (CurrentTenant.Id.HasValue)
            return CurrentTenant.Id.Value;

        var tenants = await tenantRepository.GetListAsync(cancellationToken: cancellationToken);
        var defaultTenant = tenants.OrderBy(tenant => tenant.CreationTime).FirstOrDefault();
        if (defaultTenant is not null)
            return defaultTenant.Id;

        var storedTenants = await tenantStore.GetListAsync();
        var fallbackTenant = storedTenants.FirstOrDefault(tenant => tenant.IsActive);
        if (fallbackTenant is null)
            throw new BusinessException(RegistrationExceptionCodes.TenantNotAvailable);

        return fallbackTenant.Id;
    }

    // Tenant'i acar, tum izinleri tasiyan sahip rolunu kurar ve kullaniciyi host'ta acip bu role baglar.
    public virtual async Task<RegisteredUserModel> RegisterTenantAsync(
        TenantRegistrationModel model,
        CancellationToken cancellationToken = default)
    {
        var scope = await CreateTenantScopeAsync(model.TenantName, cancellationToken);
        return await RegisterUserAsync(scope, model.UserName, model.Email, model.Password, cancellationToken);
    }

    // Kullaniciyi e-postasiyla bulur ya da host'ta sifresiz acar; tenant rollerini dogrudan verir, organizasyona bagli
    // rollerde kullaniciyi o organizasyona uye yapar. Yeni kullanici sifresini davet baglantisiyla belirler.
    public virtual async Task<InvitedUserModel> InviteUserAsync(
        InvitationModel model,
        CancellationToken cancellationToken = default)
    {
        var roles = await GetInvitableRolesAsync(model.RoleIds, cancellationToken);
        var roleOrganizations = await GetRoleOrganizationsAsync(model.RoleIds, cancellationToken);
        var (user, isNewUser) = await FindOrCreateInvitedUserAsync(model.UserName, model.Email, cancellationToken);
        await AddTenantRolesAsync(user, roles, cancellationToken);
        await AddOrganizationMembershipsAsync(user, roleOrganizations.Values, cancellationToken);
        return new InvitedUserModel
        {
            User = user,
            IsNewUser = isNewUser,
            PasswordResetToken = isNewUser ? await userManager.GeneratePasswordResetTokenAsync(user) : null
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
        IEnumerable<Guid> organizationRoleIds,
        IReadOnlyList<TenantConfiguration> tenants)
    {
        var boundRoleIds = organizationRoleIds.ToHashSet();
        return tenantIds.ConvertAll(id => new LoginTenantOptionModel
        {
            Id = id,
            Name = tenants.FirstOrDefault(tenant => tenant.Id == id)?.Name,
            HasRole = roles.Exists(role => role.TenantId == id && !boundRoleIds.Contains(role.Id))
        });
    }

    // Tenant ya da tenant'taki organizasyon birden fazla secenekken secilmediyse secim eksiktir.
    private static bool RequiresSelection(LoginScopeModel scope, Guid? tenantId, Guid? organizationId)
    {
        if (tenantId is null && scope.Tenants.Count > 1)
            return true;

        var tenant = tenantId ?? scope.Tenants.SingleOrDefault()?.Id;
        return organizationId is null && scope.Organizations.Count(organization => organization.TenantId == tenant) > 1;
    }

    // Secilen tenant kullanicinin tenant'larindan biri olmali; secilmediyse tek secenek alinir.
    private static Guid? ResolveTenant(List<Guid?> tenantIds, Guid? tenantId, bool selectSingleScope)
    {
        if (selectSingleScope && tenantId is null && tenantIds.Count <= 1)
            return tenantIds.SingleOrDefault();

        if (!tenantIds.Contains(tenantId))
            throw new BusinessException(LoginExceptionCodes.ScopeNotAvailable);

        return tenantId;
    }

    // Secilen organizasyonun ust organizasyonunu getirir; kok organizasyonda veya organizasyon secilmediyse null doner.
    private async Task<OrganizationUnit?> FindParentOrganizationAsync(
        OrganizationUnit? organization,
        CancellationToken cancellationToken = default)
    {
        return organization?.ParentId is null
            ? null
            : await organizationRepository.FindAsync(organization.ParentId.Value, includeDetails: false, cancellationToken);
    }

    // Secilen organizasyon tenant'taki uyeliklerden biri olmali; secilmediyse tek uyelik alinir.
    private static OrganizationUnit? ResolveOrganization(
        List<OrganizationUnit> organizations,
        Guid? tenantId,
        Guid? organizationId,
        bool selectSingleScope)
    {
        var candidates = organizations.FindAll(organization => organization.TenantId == tenantId);
        if (!selectSingleScope && organizationId is null)
            return null;
        if (organizationId is null && candidates.Count <= 1)
            return candidates.SingleOrDefault();

        return candidates.Find(organization => organization.Id == organizationId)
               ?? throw new BusinessException(LoginExceptionCodes.ScopeNotAvailable);
    }

    // Tenant'taki bagimsiz dogrudan rollerle secilen organizasyona bagli dogrudan rolleri toplar.
    private async Task<List<IdentityRole>> GetScopeRolesAsync(
        List<IdentityRole> directRoles,
        Guid? tenantId,
        OrganizationUnit? organization,
        CancellationToken cancellationToken = default)
    {
        var roleOrganizations = await GetRoleOrganizationsAsync(
            directRoles.Select(role => role.Id),
            cancellationToken);
        return
        [
            .. directRoles.Where(role => role.TenantId == tenantId)
                .Where(role => !roleOrganizations.TryGetValue(role.Id, out var boundOrganization)
                               || boundOrganization.Id == organization?.Id)
        ];
    }

    // Tenant ve sahip rolunu kurar; feature override'i yazmayarak ABP varsayilanlarini korur.
    private async Task<RegistrationScopeModel> CreateTenantScopeAsync(
        string tenantName,
        CancellationToken cancellationToken = default)
    {
        var tenant = await tenantRepository.InsertAsync(
            await tenantManager.CreateAsync(tenantName),
            cancellationToken: cancellationToken);
        await CreateOwnerRoleAsync(tenant.Id, cancellationToken);
        return tenant.ToRegistrationScopeModel([RegistrationConsts.OwnerRoleName]);
    }

    // Sahip rolu silinemez rol olarak acilir ve uygulamanin tenant tarafindaki tum izinlerini alir; varsayilan rol
    // degildir, tenant'a sonradan katilan kullanicilara verilmez.
    private async Task CreateOwnerRoleAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var role = new IdentityRole(GuidGenerator.Create(), RegistrationConsts.OwnerRoleName, tenantId)
            {
                IsStatic = true,
                IsPublic = true
            };
            (await roleManager.CreateAsync(role)).CheckErrors();
        }

        await permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            RegistrationConsts.OwnerRoleName,
            await GetTenantPermissionNamesAsync(cancellationToken),
            tenantId);
    }

    // Tenant tarafinda tanimli ve role verilebilen izinlerin adlarini getirir.
    private async Task<List<string>> GetTenantPermissionNamesAsync(CancellationToken cancellationToken = default)
        =>
        [
            .. (await permissionDefinitionManager.GetPermissionsAsync())
                .Where(permission => permission.MultiTenancySide.HasFlag(MultiTenancySides.Tenant))
                .Where(permission => permission.Providers.Count == 0
                                     || permission.Providers.Contains(RolePermissionValueProvider.ProviderName))
                .Select(permission => permission.Name)
        ];

    // Secilen mevcut tenant'ta kullanici tenant'in varsayilan rollerine girer.
    private async Task<RegistrationScopeModel> GetTenantScopeAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await GetActiveTenantAsync(tenantId, cancellationToken);
        return tenant.ToRegistrationScopeModel(await GetDefaultRoleNamesAsync(tenant.Id, cancellationToken));
    }

    // Organizasyon secilen tenant'ta olmali ve en az bir bagli varsayilan rolle kayda acik olmali.
    private async Task<RegistrationScopeModel> GetOrganizationScopeAsync(
        Guid tenantId,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await GetActiveTenantAsync(tenantId, cancellationToken);
        using (CurrentTenant.Change(tenant.Id))
        {
            var organization = await organizationRepository.FindAsync(organizationId, includeDetails: true, cancellationToken)
                               ?? throw new BusinessException(RegistrationExceptionCodes.OrganizationNotAvailable);
            var roles = await roleRepository.GetListAsync(organization.Roles.Select(binding => binding.RoleId), cancellationToken);
            var defaultRoles = roles.FindAll(role => role.IsDefault);
            if (defaultRoles.Count == 0)
                throw new BusinessException(RegistrationExceptionCodes.OrganizationNotOpen);

            return tenant.ToRegistrationScopeModel(
                organization,
                defaultRoles.ConvertAll(role => role.Name));
        }
    }

    // Secilen tenant var ve aktif olmali.
    private async Task<TenantConfiguration> GetActiveTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await tenantStore.FindAsync(tenantId);
        if (tenant is not null && tenant.IsActive)
            return tenant;

        var tenantEntity = await tenantRepository.FindAsync(tenantId, cancellationToken: cancellationToken);
        if (tenantEntity is null)
            throw new BusinessException(RegistrationExceptionCodes.TenantNotAvailable);

        return new TenantConfiguration(tenantEntity.Id, tenantEntity.Name);
    }

    // Tenant'in organizasyona bagli olmayan varsayilan rollerini getirir; organizasyon rolleri kendi kayit akisinda
    // dogrudan verilir. Hic yoksa tenant'in diger rollerini alir.
    private async Task<List<string>> GetDefaultRoleNamesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var defaultRoles = await roleRepository.GetDefaultOnesAsync(includeDetails: false, cancellationToken);
            var organizationRoleIds = (await organizationRepository.GetListByRoleIdsAsync(
                    defaultRoles.ConvertAll(role => role.Id),
                    cancellationToken))
                .SelectMany(organization => organization.Roles)
                .Select(binding => binding.RoleId)
                .ToHashSet();
            var tenantRoles = defaultRoles.FindAll(role => !organizationRoleIds.Contains(role.Id));
            if (tenantRoles.Count == 0)
            {
                var allRoles = await roleRepository.GetListAsync(cancellationToken: cancellationToken);
                var allOrgRoleIds = (await organizationRepository.GetListByRoleIdsAsync(
                        allRoles.ConvertAll(role => role.Id),
                        cancellationToken))
                    .SelectMany(organization => organization.Roles)
                    .Select(binding => binding.RoleId)
                    .ToHashSet();
                tenantRoles = allRoles.FindAll(role => !allOrgRoleIds.Contains(role.Id));
            }

            if (tenantRoles.Count == 0)
                throw new BusinessException(RegistrationExceptionCodes.NoDefaultRole);

            return tenantRoles.ConvertAll(role => role.Name);
        }
    }

    // Rollerin hepsi var olmali ve davet edenin kapsaminda olmali: tenant'taki davet eden sadece kendi tenant'inin
    // rollerini verir, host'taki davet eden her tenant'in rollerini verebilir.
    private async Task<List<IdentityRole>> GetInvitableRolesAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var roles = await roleRepository.GetListAsync(roleIds, cancellationToken);
        if (roles.Count != roleIds.Distinct().Count() ||
            roles.Exists(role => CurrentTenant.Id is not null && role.TenantId != CurrentTenant.Id))
            throw new BusinessException(InvitationExceptionCodes.RoleNotAvailable);

        await EnsureRolesWithinCurrentAsync(roles, cancellationToken);

        return roles;
    }

    // Organizasyona bagli rolu organizasyonuyla eslestirir; bir rol birden cok organizasyona bagliysa kullanicinin
    // hangisine uye yapilacagi belirsiz oldugu icin davet reddedilir.
    private async Task<Dictionary<Guid, OrganizationUnit>> GetRoleOrganizationsAsync(
        IEnumerable<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var ids = roleIds.Distinct().ToList();
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var organizations = await organizationRepository.GetListByRoleIdsAsync(ids, cancellationToken);
        var bindings = organizations
            .SelectMany(organization => organization.Roles
                .Where(binding => ids.Contains(binding.RoleId))
                .Select(binding => (binding.RoleId, Organization: organization)))
            .ToList();
        if (bindings.GroupBy(binding => binding.RoleId).Any(group => group.Count() > 1))
            throw new BusinessException(InvitationExceptionCodes.RoleInMultipleOrganizations);

        return bindings.ToDictionary(binding => binding.RoleId, binding => binding.Organization);
    }

    // E-postasi kayitli kullanici varsa o kullanilir; yoksa kullanici host'ta sifresiz acilir. Rol baglari host
    // kapsaminda yuklenir ki sahip oldugu roller atlanabilsin.
    private async Task<(IdentityUser User, bool IsNewUser)> FindOrCreateInvitedUserAsync(
        string userName,
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await FindHostUserByEmailAsync(email);
        if (user is not null)
        {
            using (CurrentTenant.Change(null))
            {
                await userRepository.EnsureCollectionLoadedAsync(user, item => item.Roles, cancellationToken);
            }
            return (user, false);
        }

        return (await CreateHostUserAsync(userName, email, cancellationToken), true);
    }

    // Tenant rollerini rolun kendi tenant'inda dogrudan verir; kullanicinin zaten sahip oldugu rol atlanir.
    private async Task AddTenantRolesAsync(
        IdentityUser user,
        List<IdentityRole> roles,
        CancellationToken cancellationToken = default)
    {
        foreach (var tenantRoles in roles.Where(role => !user.IsInRole(role.Id)).GroupBy(role => role.TenantId))
        {
            using (CurrentTenant.Change(tenantRoles.Key))
                (await userManager.AddToRolesAsync(user, tenantRoles.Select(role => role.Name))).CheckErrors();
        }
    }

    // Organizasyona bagli rol icin secilebilirligi saglayan uyelik kendi tenant'inda acilir, zaten uyeyse atlanir.
    private async Task AddOrganizationMembershipsAsync(
        IdentityUser user,
        IEnumerable<OrganizationUnit> organizations,
        CancellationToken cancellationToken = default)
    {
        foreach (var organization in organizations.DistinctBy(organization => organization.Id))
            await userManager.AddToOrganizationAsync(CurrentTenant, user, organization, cancellationToken);
    }

    // Kullaniciyi host'ta acar; kapsamin tenant'inda kapsamin rollerine baglar, organizasyon varsa ona uye yapar.
    private async Task<RegisteredUserModel> RegisterUserAsync(
        RegistrationScopeModel scope,
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await CreateHostUserAsync(userName, email, password, cancellationToken);
        using (CurrentTenant.Change(scope.TenantId))
        {
            if (scope.RoleNames.Count > 0)
                (await userManager.AddToRolesAsync(user, scope.RoleNames)).CheckErrors();

            if (scope.OrganizationId is not null)
            {
                var organization = await organizationRepository.GetAsync(
                    scope.OrganizationId.Value,
                    includeDetails: false,
                    cancellationToken);
                await userManager.AddToOrganizationAsync(CurrentTenant, user, organization, cancellationToken);
            }
        }

        return scope.ToRegisteredUserModel(user);
    }

    // Kullaniciyi host'ta, cercevenin sifre ve tekillik kurallariyla acar.
    private async Task<IdentityUser> CreateHostUserAsync(
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
        => await CreateHostUserAsync(userName, email, password, hasPassword: true, cancellationToken);

    // Kullaniciyi host'ta sifresiz acar; davet sonrasi sifre belirleme akisi bunu kullanir.
    private async Task<IdentityUser> CreateHostUserAsync(
        string userName,
        string email,
        CancellationToken cancellationToken = default)
        => await CreateHostUserAsync(userName, email, null, hasPassword: false, cancellationToken);

    // Host kullanicisi kurulumunun ortak Identity ayari ve olusturma adimlarini tek yerde isletir.
    private async Task<IdentityUser> CreateHostUserAsync(
        string userName,
        string email,
        string? password,
        bool hasPassword,
        CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
        {
            await identityOptions.SetAsync();
            var user = new IdentityUser(GuidGenerator.Create(), userName, email);
            var result = hasPassword
                ? await userManager.CreateAsync(user, password!)
                : await userManager.CreateAsync(user);
            result.CheckErrors();
            return user;
        }
    }

    // Host kullanicisini e-postasiyla tenant filtresinden bagimsiz bulur.
    private async Task<IdentityUser?> FindHostUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(null))
            return await userManager.FindByEmailAsync(email);
    }

    // Kullanicinin oturum tenant'indaki organizasyon uyeliklerini getirir.
    private async Task<List<OrganizationModel>> GetUserOrganizationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var organizations = await organizationRepository.GetUserModelListAsync(userId, cancellationToken);
        return organizations.FindAll(organization => organization.TenantId == CurrentTenant.Id);
    }

    // Dogrulanmis secimden seviye uretir; host'a bagli organizasyon da organizasyon kapsamidir.
    private static ScopeLevel ResolveScopeLevel(Guid? tenantId, OrganizationUnit? organization)
    {
        if (organization is not null)
            return organization.ParentId is null ? ScopeLevel.Organization : ScopeLevel.SubOrganization;
        return tenantId is null ? ScopeLevel.SuperAdmin : ScopeLevel.Tenant;
    }

    // Rolleri tek organizasyon sorgusuyla seviyeli oturum modellerine donusturur.
    private async Task<List<CurrentLoginRoleModel>> BuildRoleModelsAsync(
        List<IdentityRole> roles,
        CancellationToken cancellationToken = default)
    {
        var organizations = await GetRoleOrganizationsAsync(roles.Select(role => role.Id), cancellationToken);
        return roles.ConvertAll(role => role.ToCurrentLoginRoleModel(
            ResolveScopeLevel(role.TenantId, organizations.GetValueOrDefault(role.Id))));
    }

    // Organizasyon kapsamindaki davet rollerini mevcut organizasyon veya alt agaciyla sinirlar.
    private async Task EnsureRolesWithinCurrentAsync(
        List<IdentityRole> roles,
        CancellationToken cancellationToken = default)
    {
        if (!currentOrganization.IsAvailable)
            return;
        var organizations = await GetRoleOrganizationsAsync(roles.Select(role => role.Id), cancellationToken);
        if (organizations.Count != roles.Count)
            throw new BusinessException(InvitationExceptionCodes.RoleNotAvailable);
        var current = await organizationRepository.GetAsync(currentOrganization.Id!.Value, false, cancellationToken);
        if (organizations.Values.Any(organization => !organization.IsWithinScopeOf(current)))
            throw new BusinessException(OrganizationExceptionCodes.OutsideScope);
    }

    // Davet tokenini istemcinin sifre belirleme sayfasina yonlendiren linke donusturur.
    public virtual string BuildInvitationLink(
        InvitedUserModel invitation,
        string resetPasswordUrl,
        string? returnUrl,
        string? returnUrlHash)
    {
        var link = $"{resetPasswordUrl}?userId={invitation.UserId}&{TenantResolverConsts.DefaultTenantKey}=" +
                   $"&resetToken={UrlEncoder.Default.Encode(invitation.PasswordResetToken!)}";
        if (!string.IsNullOrEmpty(returnUrl))
            link += "&returnUrl=" + NormalizeReturnUrl(returnUrl);
        if (!string.IsNullOrEmpty(returnUrlHash))
            link += "&returnUrlHash=" + returnUrlHash;
        return link;
    }

    // OpenIddict donus adreslerini ABP AccountEmailer ile ayni bicimde normalize eder.
    private static string NormalizeReturnUrl(string returnUrl)
    {
        if (returnUrl.StartsWith("/connect/authorize/callback", StringComparison.OrdinalIgnoreCase) &&
            returnUrl.Contains('?'))
        {
            foreach (var pair in returnUrl.Split('?')[1].Split('&'))
            {
                var parts = pair.Split('=');
                if (parts.Length > 1 && parts[0] == "redirect_uri")
                    return HttpUtility.UrlDecode(parts[1]);
            }
        }

        return returnUrl.StartsWith("/connect/authorize?", StringComparison.OrdinalIgnoreCase)
            ? HttpUtility.UrlEncode(returnUrl)
            : returnUrl;
    }
}
