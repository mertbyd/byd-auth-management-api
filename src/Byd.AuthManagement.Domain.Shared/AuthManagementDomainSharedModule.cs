using Byd.CoreExtensions;
using Byd.LogManagement;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Byd.AuthManagement.Localization;
using Volo.Abp.Domain;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Byd.AuthManagement;

[DependsOn(
    typeof(LogManagementDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpValidationModule),
    typeof(AbpDddDomainSharedModule)
)]
public class AuthManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AuthManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<AuthManagementResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/AuthManagement");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("AuthManagement", typeof(AuthManagementResource));
        });
    }
}
