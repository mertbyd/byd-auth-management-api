using Byd.CoreExtensions;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpDddDomainModule),
    typeof(AuthManagementDomainSharedModule)
)]
public class AuthManagementDomainModule : AbpModule
{

}
