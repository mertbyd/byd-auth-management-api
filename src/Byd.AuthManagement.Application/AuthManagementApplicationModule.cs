using System.Net.Http;
using Duende.IdentityModel.Client;
using Byd.CoreExtensions;
using Byd.LogManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Byd.AuthManagement.Logins;
using Byd.AuthManagement.Options;
using Volo.Abp.Account;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Application;
using Volo.Abp.FeatureManagement;
using Volo.Abp.TenantManagement;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(CoreExtensionsApplicationModule),
    typeof(LogManagementApplicationModule),
    typeof(AuthManagementDomainModule),
    typeof(AuthManagementApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpMapperlyModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpOpenIddictAspNetCoreModule),
    typeof(AbpAccountApplicationModule)
    )]
public class AuthManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<AuthManagementApplicationModule>();
        context.Services.AddHttpClient();

        // Token ucunun adresi token sunucusunun discovery dokumanindan okunur; dokuman bir kez cekilip saklanir.
        context.Services.AddSingleton(provider => new DiscoveryCache(
            provider.GetRequiredService<IOptions<AuthManagementModuleOptions>>().Value.Authority,
            () => provider.GetRequiredService<IHttpClientFactory>().CreateClient()));

        // Kapsam handler'i cercevenin handler'indan once calisir; claim'lerin token'a yazilip yazilmayacagini o belirler.
        Configure<AbpOpenIddictClaimsPrincipalOptions>(options =>
        {
            options.ClaimsPrincipalHandlers.Insert(0, typeof(LoginScopeClaimsPrincipalHandler));
        });
    }
}
