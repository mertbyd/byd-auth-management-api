using Volo.Abp.Modularity;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(AuthManagementDomainModule),
    typeof(AuthManagementTestBaseModule)
)]
public class AuthManagementDomainTestModule : AbpModule
{

}
