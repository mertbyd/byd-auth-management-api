using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(AuthManagementApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class AuthManagementHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(AuthManagementApplicationContractsModule).Assembly,
            AuthManagementRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AuthManagementHttpApiClientModule>();
        });

    }
}
