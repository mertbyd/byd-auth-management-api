using Byd.AuthManagement.Consts;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace Byd.AuthManagement.Extensions;

/// <summary>
/// Modulun devraldigi ABP tablolarinin sema ve tablo on ekini modulun sozlesmesine gore ezer.
/// </summary>
// islevi: Devralinan ABP modullerinin DbProperties degerlerini model kurulmadan once auth modulunun semasina
// ve bos on eke yonlendirir.
// sistemdeki gorevi: Izin, kiraci, kimlik ve OpenIddict tablolari auth modulunun verisidir; core ve log
// modullerinin kendi semalari gibi bu tablolar da auth_management semasinda durur ve adlari modulun
// migration'indaki adlarla ayni kalir. Karar tuketen host'a birakilmaz: ABP bu degerleri sinifa ilk erisildigi
// anda AbpCommonDbProperties'ten kopyalar; host sinifa erken erisirse on ek "Abp" olarak kalir ve modulun
// context'i migration'in kurmadigi bir tabloya yazmaya calisir.
public static class ServiceConfigurationContextExtension
{
    // Devralinan tablolari auth modulunun semasina alir; tablo on ekini kaldirir.
    public static void ResolveSchemaAndPrefix()
    {
        AbpIdentityDbProperties.DbSchema = DatabaseConsts.ModuleSchema;
        AbpIdentityDbProperties.DbTablePrefix = string.Empty;

        AbpOpenIddictDbProperties.DbSchema = DatabaseConsts.ModuleSchema;
        AbpOpenIddictDbProperties.DbTablePrefix = string.Empty;

        AbpPermissionManagementDbProperties.DbSchema = DatabaseConsts.ModuleSchema;
        AbpPermissionManagementDbProperties.DbTablePrefix = string.Empty;

        AbpFeatureManagementDbProperties.DbSchema = DatabaseConsts.ModuleSchema;
        AbpFeatureManagementDbProperties.DbTablePrefix = string.Empty;

        AbpTenantManagementDbProperties.DbSchema = DatabaseConsts.ModuleSchema;
        AbpTenantManagementDbProperties.DbTablePrefix = string.Empty;
    }
}
