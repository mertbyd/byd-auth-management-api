using Byd.CoreExtensions.EntityFrameworkCore;
using Byd.LogManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Byd.AuthManagement.Extensions;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Byd.AuthManagement.EntityFrameworkCore;

[DependsOn(
    typeof(CoreExtensionsEntityFrameworkCoreModule),
    typeof(LogManagementEntityFrameworkCoreModule),
    typeof(AuthManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule)
)]
// islevi: Auth DbContext'ini PostgreSQL saglayicisiyla ABP'ye kaydeder.
// sistemdeki gorevi: Auth entity ve repository altyapisinin EF Core kompozisyon kokudur.
public class AuthManagementEntityFrameworkCoreModule : AbpModule
{
    // Sema cozumu modelden once calismak zorunda; ABP sema degerlerini sinifa ilk erisimde kopyalar.
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ServiceConfigurationContextExtension.ResolveSchemaAndPrefix();
    }

    // PostgreSQL saglayicisi yalniz auth DbContext'ine baglanir; tuketen host kendi context'ini yonetir.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<AuthManagementDbContext>(configurationContext =>
            {
                configurationContext.UseNpgsql();
            });
        });

        context.Services.AddAbpDbContext<AuthManagementDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
