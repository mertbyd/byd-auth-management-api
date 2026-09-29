using Byd.CoreExtensions;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpDddDomainModule),
    typeof(AuthManagementDomainSharedModule)
)]
public class AuthManagementDomainModule : AbpModule
{

}
