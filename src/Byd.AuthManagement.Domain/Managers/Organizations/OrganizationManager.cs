using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Models.Organizations;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Byd.AuthManagement.Managers.Organizations;

/// <summary>
/// Organizasyonun olusturma, guncelleme ve uyelik okuma kurallarini yonetir.
/// </summary>
// islevi: Yeni organizasyonu istegin tenant'inda kurar; guncellemede once yeri, sonra adi degistirir; uyelikleri okur.
// sistemdeki gorevi: Agac kurallarini (kod, tekillik, tasima) cercevenin Manager'ina birakan domain sahibidir.
public class OrganizationManager(
    IOrganizationRepository repository,
    OrganizationUnitManager organizationUnitManager,
    IDataFilter dataFilter) : DomainService
{
    // Kullanicinin istegin tenant'indaki uyeliklerini getirir; uyelik kaydi kullanicinin kendi tenant'inda durabilir.
    public virtual async Task<List<OrganizationModel>> GetUserOrganizationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var tenantFilter = dataFilter.Disable<IMultiTenant>();
        var organizations = await repository.GetUserModelListAsync(userId, cancellationToken);
        return organizations.FindAll(organization => organization.TenantId == CurrentTenant.Id);
    }

    // Create modelinden istegin tenant'inda yeni organizasyon kurup agaca ekler.
    public virtual async Task<OrganizationUnit> CreateAsync(
        OrganizationCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = new OrganizationUnit(GuidGenerator.Create(), model.DisplayName, model.ParentId, CurrentTenant.Id);
        await organizationUnitManager.CreateAsync(entity);
        return entity;
    }

    // Ust kayit degistiyse kaydi alt agaciyla tasir, sonra adini gunceller.
    public virtual async Task UpdateAsync(
        Guid id,
        OrganizationUpdateModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(id, includeDetails: false, cancellationToken);
        if (entity.ParentId != model.ParentId)
            await organizationUnitManager.MoveAsync(id, model.ParentId);

        entity.DisplayName = model.DisplayName;
        await organizationUnitManager.UpdateAsync(entity);
    }
}
