using Byd.CoreExtensions;
using Byd.LogManagement;
using Byd.AuthManagement.Consts;
using Volo.Abp.Domain;
using Byd.AuthManagement.Providers.Features;
using Byd.AuthManagement.Providers.Settings;
using Byd.AuthManagement.Permissions;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(LogManagementDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpDddDomainModule),
    typeof(AuthManagementDomainSharedModule)
)]
public class AuthManagementDomainModule : AbpModule
{
    // Dinamik tanimlari veritabanindan okumayi ve organizasyon feature/ayar provider'larini etkinlestirir.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.IsDynamicFeatureStoreEnabled = true;
            options.Providers.Add<OrganizationFeatureManagementProvider>();
            options.ProviderPolicies[FeatureConsts.OrganizationProviderName] =
                AuthManagementPermissions.Features.Manage;
        });

        Configure<AbpFeatureOptions>(options =>
        {
            // FeatureChecker provider listesini ters sirada calistirir; son eklenen organizasyon kurali once sorulur.
            options.ValueProviders.Add<OrganizationFeatureValueProvider>();
        });

        // SettingProvider listeyi ters sirada calistirir; organizasyon degeri kullanicidan sonra, tenant'tan once sorulur.
        Configure<AbpSettingOptions>(options => options.ValueProviders.Insert(
            options.ValueProviders.IndexOf(typeof(TenantSettingValueProvider)) + 1,
            typeof(OrganizationSettingValueProvider)));
        Configure<SettingManagementOptions>(options => options.Providers.Insert(
            options.Providers.IndexOf(typeof(TenantSettingManagementProvider)) + 1,
            typeof(OrganizationSettingManagementProvider)));
    }
}
