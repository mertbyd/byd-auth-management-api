using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Byd.AuthManagement.Consts;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp.Settings;

namespace Byd.AuthManagement.Providers.Settings;

/// <summary>
/// Ayar degerini aktif organizasyon kapsamina gore okur.
/// </summary>
// islevi: organization_id varsa O provider degerini okur, yoksa ABP'nin sonraki provider'ina gecis verir.
// sistemdeki gorevi: Tenant ayar provider'inin ABP-native davranisini organizasyon kapsamina tasir; organizasyon degeri
// tanimli degilse tenant, global ve varsayilan deger sirayla kullanilir.
public class OrganizationSettingValueProvider(
    ISettingStore settingStore,
    ICurrentOrganization currentOrganization)
    : SettingValueProvider(settingStore)
{
    public override string Name => SettingProviderConsts.OrganizationProviderName;

    // Organizasyon yoksa zinciri kesmez; varsa degeri ABP store sozlesmesiyle okur.
    public override Task<string?> GetOrNullAsync(SettingDefinition setting)
        => currentOrganization.Id.HasValue
            ? SettingStore.GetOrNullAsync(setting.Name, Name, currentOrganization.Id.Value.ToString())
            : Task.FromResult<string?>(null);

    // Organizasyon yoksa bos degerler doner; varsa butun ayarlar tek store cagrisiyla okunur.
    public override Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
        => currentOrganization.Id.HasValue
            ? SettingStore.GetAllAsync(settings.Select(setting => setting.Name).ToArray(), Name,
                currentOrganization.Id.Value.ToString())
            : Task.FromResult(settings.Select(setting => new SettingValue(setting.Name, null)).ToList());
}
