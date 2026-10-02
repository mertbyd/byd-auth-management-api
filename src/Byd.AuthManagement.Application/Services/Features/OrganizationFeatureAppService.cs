using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Managers.Organizations;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;

namespace Byd.AuthManagement.Services.Features;

// islevi: ABP feature yonetim politikasina organizasyon kapsam denetimini ekler.
// sistemdeki gorevi: Hazir listeleme, guncelleme ve sifirlama API'lerini ayni yetki sinirinda tutar.
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IFeatureAppService), typeof(FeatureAppService), typeof(OrganizationFeatureAppService))]
[RemoteService(IsEnabled = false)]
public class OrganizationFeatureAppService(
    IFeatureManager featureManager,
    IFeatureDefinitionManager featureDefinitionManager,
    IOptions<FeatureManagementOptions> options)
    : FeatureAppService(featureManager, featureDefinitionManager, options)
{
    private OrganizationManager OrganizationManager =>
        LazyServiceProvider.LazyGetRequiredService<OrganizationManager>();

    // Standart policy basarili olduktan sonra O anahtarinin aktif tenant ve agacta oldugunu denetler.
    protected override async Task CheckProviderPolicy(string providerName, string providerKey)
    {
        await base.CheckProviderPolicy(providerName, providerKey);
        if (providerName == FeatureConsts.OrganizationProviderName)
            await OrganizationManager.EnsureCanManageFeaturesAsync(providerKey);
    }
}
