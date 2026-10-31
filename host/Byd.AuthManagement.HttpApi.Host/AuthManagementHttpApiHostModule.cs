using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Validation.AspNetCore;
using System.Reflection;
using Byd.CoreExtensions;
using Byd.CoreExtensions.Extensions;
using Byd.CoreExtensions.Options;
using Byd.FileManagement;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.EntityFrameworkCore;
using Byd.AuthManagement.MultiTenancy;
using Byd.AuthManagement.Options;
using StackExchange.Redis;
using Microsoft.OpenApi;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(AuthManagementApplicationModule),
    typeof(AuthManagementEntityFrameworkCoreModule),
    typeof(AuthManagementHttpApiModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class AuthManagementHttpApiHostModule : AbpModule
{
    // ABP ortak ve ayar tablolarini core'un beyan ettigi semaya alir; model kurulmadan once calisir.
    // Token'lar bu host'ta uretildigi icin dogrulama da yerel sunucuya baglanir; disaridaki bir adrese gidilmez.
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ServiceConfigurationContextExtension.ResolveSchemaAndPrefix();

        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences(OpenIddictConsts.ApiScope);
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        // Gelistirme sertifikasi yalniz gelistirmede kalir; diger ortamlar imza ve sifreleme anahtarini dagitimdaki
        // sertifikadan alir ve issuer'i config'teki adrese sabitler.
        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate(
                    "openiddict.pfx",
                    configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    // Core modul adi ve ortam adini beyan eder; core saglayicilari bu beyan olmadan acilista durur.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        Configure<CoreExtensionsModuleOptions>(options =>
        {
            options.ModuleName = Assembly.GetEntryAssembly()?.GetName().Name?
                .Replace(".HttpApi.Host", string.Empty);
            options.EnvironmentName = hostingEnvironment.EnvironmentName;
        });

        // Login servisi token'i bu adresteki token ucundan ister; adres host'un kendi ayarindan gelir.
        Configure<AuthManagementModuleOptions>(options =>
        {
            options.Authority = configuration["AuthServer:Authority"]!;
        });

        // Sifre sifirlama baglantisi istemcinin sifre sayfasina gider; istemcinin adresi host'un ayarindan gelir.
        Configure<AppUrlOptions>(options =>
        {
            options.Applications[PasswordConsts.ClientAppName].RootUrl = configuration["App:ClientUrl"];
            options.Applications[PasswordConsts.ClientAppName].Urls[AccountUrlNames.PasswordReset] =
                PasswordConsts.ResetPasswordPagePath;
        });

        // Host yalniz bearer tabanli API sunar; cookie tabanli CSRF dogrulamasi Swagger POST isteklerine uygulanmaz.
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<AuthManagementDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Byd.AuthManagement.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<AuthManagementDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Byd.AuthManagement.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<AuthManagementApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Byd.AuthManagement.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<AuthManagementApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Byd.AuthManagement.Application", Path.DirectorySeparatorChar)));
            });
        }

        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                {"AuthManagement", "AuthManagement API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "AuthManagement API", Version = "v1"});
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                // Hesap login ucunun verdigi access token Swagger'da elle girilir; OAuth kod akisinin giris sayfasi yoktur.
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference("Bearer", document), [] }
                });
            });

        // Modul yalniz Ingilizce ve Turkce kaynak tasir; beyan edilen dil listesi de bu ikisiyle sinirlidir.
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
        });

        // Bearer tasiyan istekler OpenIddict dogrulamasina yonlenir; token'i ureten sunucu ayni host'tadir.
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "AuthManagement:";
        });

        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("AuthManagement");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "AuthManagement-Protection-Keys");
        }

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        app.UseAbpRequestLocalization();
        // Organizasyon feature'i authorization sirasinda degerlendirilir; claim kapsami once kurulmalidir.
        app.UseCoreExtensions();//core module
        app.UseFileManagement();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("AuthManagement");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }

    // Modullerin seed katkilarini (token client'i ve scope'u vb.) uygulama acilirken idempotent olarak calistirir.
    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync();
    }
}
