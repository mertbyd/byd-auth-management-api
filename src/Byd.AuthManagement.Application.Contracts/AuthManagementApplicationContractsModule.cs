using Byd.CoreExtensions;
using Byd.LogManagement;
using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;
using Volo.Abp.FeatureManagement;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationContractsModule),
    typeof(LogManagementApplicationContractsModule),
    typeof(AuthManagementDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class AuthManagementApplicationContractsModule : AbpModule
{

}
