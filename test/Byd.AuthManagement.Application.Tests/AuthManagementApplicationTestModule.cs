using Volo.Abp.Modularity;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(AuthManagementApplicationModule),
    typeof(AuthManagementDomainTestModule)
    )]
public class AuthManagementApplicationTestModule : AbpModule
{

}
