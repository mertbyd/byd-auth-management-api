using Byd.CoreExtensions;
using Localization.Resources.AbpUi;
using Byd.AuthManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsHttpApiModule),
    typeof(AuthManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class AuthManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(AuthManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AuthManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
