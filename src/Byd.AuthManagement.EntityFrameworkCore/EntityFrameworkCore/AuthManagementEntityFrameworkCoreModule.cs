using Byd.CoreExtensions.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace Byd.AuthManagement.EntityFrameworkCore;

[DependsOn(
    typeof(CoreExtensionsEntityFrameworkCoreModule),
    typeof(AuthManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule)
)]
// islevi: Auth DbContext'ini PostgreSQL saglayicisiyla ABP'ye kaydeder.
// sistemdeki gorevi: Auth entity ve repository altyapisinin EF Core kompozisyon kokudur.
public class AuthManagementEntityFrameworkCoreModule : AbpModule
{
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
