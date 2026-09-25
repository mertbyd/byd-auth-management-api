using Byd.CoreExtensions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationModule),
    typeof(AuthManagementDomainModule),
    typeof(AuthManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule)
    )]
public class AuthManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<AuthManagementApplicationModule>();
    }
}
