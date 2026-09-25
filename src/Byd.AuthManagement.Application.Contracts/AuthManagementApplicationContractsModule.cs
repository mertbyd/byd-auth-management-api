using Byd.CoreExtensions;
using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationContractsModule),
    typeof(AuthManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class AuthManagementApplicationContractsModule : AbpModule
{

}
