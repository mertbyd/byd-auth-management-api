using Byd.CoreExtensions;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsDomainModule),
    typeof(AbpDddDomainModule),
    typeof(AuthManagementDomainSharedModule)
)]
public class AuthManagementDomainModule : AbpModule
{

}
