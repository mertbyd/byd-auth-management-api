using System;
using System.Threading.Tasks;
using Byd.AuthManagement.Consts;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.FeatureManagement;

namespace Byd.AuthManagement.Providers.Features;

/// <summary>
/// Feature degerlerini organizasyon kimligiyle yonetir.
/// </summary>
// islevi: FeatureManagement yazma ve okuma isteklerini "O" provider'i ve organizasyon anahtarina yonlendirir.
// sistemdeki gorevi: ABP'nin yonetim zincirine organizasyon seviyesini ekleyen resmi uzatma noktasi uygulamasidir.
public class OrganizationFeatureManagementProvider(
    IFeatureManagementStore store,
    ICurrentOrganization currentOrganization)
    : FeatureManagementProvider(store), ITransientDependency
{
    public override string Name => FeatureConsts.OrganizationProviderName;

    // Alt provider degeri hesaplanirken hedef organizasyonu gecici istek kapsami yapar.
    public override Task<IAsyncDisposable> HandleContextAsync(string providerName, string providerKey)
    {
        if (providerName != Name || !Guid.TryParse(providerKey, out var organizationId))
            return base.HandleContextAsync(providerName, providerKey);
        var scope = currentOrganization.Change(organizationId);
        return Task.FromResult<IAsyncDisposable>(new AsyncDisposeFunc(() =>
        {
            scope.Dispose();
            return Task.CompletedTask;
        }));
    }

    // Provider key verilmediyse istegin organization_id kapsamindaki kimligi kullanir.
    protected override Task<string> NormalizeProviderKeyAsync(string providerKey)
        => Task.FromResult((providerKey is null
            ? currentOrganization.Id?.ToString()
            : Guid.Parse(providerKey).ToString())!);
}
