using Byd.AuthManagement.Consts;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SettingManagement;

namespace Byd.AuthManagement.Providers.Settings;

/// <summary>
/// Ayar degerlerini organizasyon kimligiyle yonetir.
/// </summary>
// islevi: SettingManager yazma ve okuma isteklerini "O" provider'i ve organizasyon anahtarina yonlendirir.
// sistemdeki gorevi: ABP'nin ayar yonetim zincirine organizasyon seviyesini ekleyen resmi uzatma noktasi uygulamasidir.
public class OrganizationSettingManagementProvider(
    ISettingManagementStore settingManagementStore,
    ICurrentOrganization currentOrganization)
    : SettingManagementProvider(settingManagementStore), ITransientDependency
{
    public override string Name => SettingProviderConsts.OrganizationProviderName;

    // Provider key verilmediyse istegin organization_id kapsamindaki kimligi kullanir.
    protected override string? NormalizeProviderKey(string? providerKey)
        => providerKey ?? currentOrganization.Id?.ToString();
}
