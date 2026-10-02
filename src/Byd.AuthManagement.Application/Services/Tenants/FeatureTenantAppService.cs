using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Byd.AuthManagement.Managers.Features;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Features;
using Volo.Abp.TenantManagement;

namespace Byd.AuthManagement.Services.Tenants;

// islevi: ABP tenant olusturma ve guncellemesine istege bagli feature secimini ekler.
// sistemdeki gorevi: Standart tenant endpointini, yetkilerini ve temel yasam dongusunu korur.
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ITenantAppService), typeof(TenantAppService), typeof(FeatureTenantAppService))]
[RemoteService(IsEnabled = false)]
public class FeatureTenantAppService(
    ITenantRepository tenantRepository,
    ITenantManager tenantManager,
    IDataSeeder dataSeeder,
    IDistributedEventBus distributedEventBus,
    ILocalEventBus localEventBus)
    : TenantAppService(tenantRepository, tenantManager, dataSeeder, distributedEventBus, localEventBus)
{
    private FeatureSelectionManager SelectionManager =>
        LazyServiceProvider.LazyGetRequiredService<FeatureSelectionManager>();

    // Temel kurulumu korur; secim varsa ABP'ye yazar, yoksa feature varsayilanlarini korur.
    [Authorize(TenantManagementPermissions.Tenants.Create)]
    public override async Task<TenantDto> CreateAsync(TenantCreateDto input)
    {
        var features = await SelectionManager.ReadFeaturesAsync(input.ExtraProperties);
        if (features is not null)
            await AuthorizationService.CheckAsync(TenantManagementPermissions.Tenants.ManageFeatures);
        var tenant = await base.CreateAsync(input);
        await SelectionManager.ReplaceAsync(TenantFeatureValueProvider.ProviderName, tenant.Id.ToString(), features);
        return tenant;
    }

    // Alan gonderilmediginde feature'lara dokunmadan standart tenant guncellemesini yurutur.
    [Authorize(TenantManagementPermissions.Tenants.Update)]
    public override async Task<TenantDto> UpdateAsync(Guid id, TenantUpdateDto input)
    {
        var features = await SelectionManager.ReadFeaturesAsync(input.ExtraProperties);
        if (features is not null)
            await AuthorizationService.CheckAsync(TenantManagementPermissions.Tenants.ManageFeatures);
        var tenant = await base.UpdateAsync(id, input);
        await SelectionManager.ReplaceAsync(TenantFeatureValueProvider.ProviderName, id.ToString(), features);
        return tenant;
    }
}
