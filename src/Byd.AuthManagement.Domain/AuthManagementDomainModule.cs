using Byd.CoreExtensions;
using Byd.AuthManagement.Consts;
using Volo.Abp.Domain;
using Byd.AuthManagement.Providers.Features;
using Byd.AuthManagement.Permissions;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsDomainModule),
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
    // Dinamik tanimlari veritabanindan okumayi ve organizasyon provider'larini etkinlestirir.
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
    }
}
