using Byd.CoreExtensions;
using Localization.Resources.AbpUi;
using Byd.AuthManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Microsoft.Extensions.DependencyInjection;

namespace Byd.AuthManagement;

// Token ucu bu modulle gelir; sifre dogrulamasi icin Identity'nin ASP.NET Core modulu dogrudan baglanir.
[DependsOn(
    typeof(CoreExtensionsHttpApiModule),
    typeof(AuthManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpOpenIddictAspNetCoreModule))]
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
