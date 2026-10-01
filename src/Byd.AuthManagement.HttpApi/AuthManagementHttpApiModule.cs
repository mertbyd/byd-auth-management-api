using Byd.CoreExtensions;
using Byd.CoreExtensions.Options;
using Localization.Resources.AbpUi;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using OpenIddict.Validation;
using Volo.Abp.OpenIddict;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.FeatureManagement;

namespace Byd.AuthManagement;

// Token ucu bu modulle gelir; sifre dogrulamasi icin Identity'nin ASP.NET Core modulu dogrudan baglanir.
[DependsOn(
    typeof(CoreExtensionsHttpApiModule),
    typeof(AuthManagementApplicationContractsModule),
    typeof(AbpFeatureManagementHttpApiModule),
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
        // Logout'ta iptal edilen oturumun token'i reddedilsin diye dogrulama token ve yetkilendirme kaydini okur.
        context.Services.PostConfigure<OpenIddictValidationOptions>(options =>
        {
            options.EnableTokenEntryValidation = true;
            options.EnableAuthorizationEntryValidation = true;
        });

        // Login, switch, refresh, logout ve token ucu ayni govdeyle mesru olarak tekrar cagrilir; kapsamlar arasinda
        // gidip gelinir, refresh token kisa sure tekrar kullanilabilir, logout'un govdesi bostur. Mukerrer istek
        // korumasi bu uclara uygulanmaz.
        Configure<CoreExtensionsModuleOptions>(options =>
        {
            options.ExcludePaths.Add("/" + AccountRoutes.Root + "/" + AccountRoutes.Login);
            options.ExcludePaths.Add("/" + AccountRoutes.Root + "/" + AccountRoutes.Switch);
            options.ExcludePaths.Add("/" + AccountRoutes.Root + "/" + AccountRoutes.Refresh);
            options.ExcludePaths.Add("/" + AccountRoutes.Root + "/" + AccountRoutes.Logout);
            options.ExcludePaths.Add("/" + OpenIddictConsts.TokenEndpointPath);
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AuthManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
