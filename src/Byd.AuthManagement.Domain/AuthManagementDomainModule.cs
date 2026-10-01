using Byd.CoreExtensions;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpDddDomainModule),
    typeof(AuthManagementDomainSharedModule)
)]
public class AuthManagementDomainModule : AbpModule
{

}
