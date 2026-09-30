using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Byd.AuthManagement.Consts;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Byd.AuthManagement.Data;

/// <summary>
/// Yapilandirilmissa host kapsamindaki super admin hesabini idempotent olarak hazirlar.
/// </summary>
// islevi: SuperAdmin rolunu acar, host izinlerini role verir ve yapilandirilan kullaniciyi bu role baglar.
// sistemdeki gorevi: Temiz veritabaninda ABP tenant, identity ve permission yonetim uclarinin ilk yetkili hesabini saglar.
public class SuperAdminDataSeeder(
    IConfiguration configuration,
    IdentityRoleManager roleManager,
    IdentityUserManager userManager,
    IPermissionDefinitionManager permissionDefinitionManager,
    IPermissionDataSeeder permissionDataSeeder,
    ICurrentTenant currentTenant)
    : DomainService, IDataSeedContributor, ITransientDependency
{
    // Config eksikse guvensiz varsayilan hesap acmaz; tam ise rol, izin ve kullaniciyi tekrar calisabilir bicimde kurar.
    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var userName = configuration[SuperAdminConsts.UserNameConfigurationKey];
        var email = configuration[SuperAdminConsts.EmailConfigurationKey];
        var password = configuration[SuperAdminConsts.PasswordConfigurationKey];
        if (string.IsNullOrWhiteSpace(userName)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
            return;

        using (currentTenant.Change(null))
        {
            var role = await roleManager.FindByNameAsync(SuperAdminConsts.RoleName);
            if (role is null)
            {
                role = new IdentityRole(GuidGenerator.Create(), SuperAdminConsts.RoleName)
                {
                    IsStatic = true,
                    IsPublic = true
                };
                (await roleManager.CreateAsync(role)).CheckErrors();
            }

            var permissionNames = (await permissionDefinitionManager.GetPermissionsAsync())
                .Where(permission => permission.MultiTenancySide.HasFlag(MultiTenancySides.Host))
                .Where(permission => permission.Providers.Count == 0
                                     || permission.Providers.Contains(RolePermissionValueProvider.ProviderName))
                .Select(permission => permission.Name)
                .ToArray();
            await permissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                SuperAdminConsts.RoleName,
                permissionNames);

            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
            {
                user = new IdentityUser(GuidGenerator.Create(), userName, email);
                user.SetEmailConfirmed(true);
                (await userManager.CreateAsync(user, password)).CheckErrors();
            }

            if (!await userManager.IsInRoleAsync(user, SuperAdminConsts.RoleName))
                (await userManager.AddToRoleAsync(user, SuperAdminConsts.RoleName)).CheckErrors();
        }
    }
}
