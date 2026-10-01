using System.Threading.Tasks;
using Byd.AuthManagement.Consts;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp.Features;

namespace Byd.AuthManagement.Providers.Features;

/// <summary>
/// Feature degerini aktif organizasyon kapsamina gore okur.
/// </summary>
// islevi: organization_id varsa O provider degerini okur, yoksa ABP'nin sonraki provider'ina gecis verir.
// sistemdeki gorevi: Tenant provider'in ABP-native davranisini organizasyon kapsamina tasir.
public class OrganizationFeatureValueProvider(
    IFeatureStore featureStore,
    ICurrentOrganization currentOrganization)
    : FeatureValueProvider(featureStore)
{
    public override string Name => FeatureConsts.OrganizationProviderName;

    // Organizasyon yoksa zinciri kesmez; varsa degeri ABP store sozlesmesiyle okur.
    public override Task<string?> GetOrNullAsync(FeatureDefinition feature)
        => currentOrganization.Id.HasValue
            ? FeatureStore.GetOrNullAsync(feature.Name, Name, currentOrganization.Id.Value.ToString())
            : Task.FromResult<string?>(null);
}
