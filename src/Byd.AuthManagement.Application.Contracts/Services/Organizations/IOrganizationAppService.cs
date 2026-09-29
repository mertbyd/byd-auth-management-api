using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Dtos.Organizations;
using Volo.Abp.Application.Services;

namespace Byd.AuthManagement.Services.Organizations;

/// <summary>
/// Organizasyon agaci kullanim senaryolarini tanimlar.
/// </summary>
// islevi: Organizasyon okuma, agac gezinme, olusturma, guncelleme ve silme kontratini acar.
// sistemdeki gorevi: Controller ile Application implementasyonu arasindaki public servis siniridir.
public interface IOrganizationAppService : IApplicationService
{
    // Kimligi verilen organizasyonu getirir.
    Task<OrganizationDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    // Ust organizasyonu olmayan kayitlari getirir.
    Task<List<OrganizationDto>> GetRootsAsync(CancellationToken cancellationToken = default);

    // Kimligi verilen organizasyonun dogrudan alt kayitlarini getirir.
    Task<List<OrganizationDto>> GetChildrenAsync(Guid id, CancellationToken cancellationToken = default);

    // Tenant altindaki tum organizasyonlari getirir.
    Task<List<OrganizationDto>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    // Tenant altindaki kok organizasyonlari getirir.
    Task<List<OrganizationDto>> GetTenantRootsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    // Tenant organizasyonlarini hiyerarsik agac kodu sirasinda getirir.
    Task<List<OrganizationDto>> GetTenantTreeAsync(Guid tenantId, CancellationToken cancellationToken = default);

    // Kullanicinin tenant icindeki organizasyon uyeliklerini getirir.
    Task<List<OrganizationDto>> GetUserOrganizationsAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    // Dogrulanmis girdiden yeni organizasyon olusturur.
    Task<OrganizationDto> CreateAsync(
        CreateOrganizationDto input,
        CancellationToken cancellationToken = default);

    // Kimligi verilen organizasyonun adini ve agactaki yerini gunceller.
    Task<OrganizationDto> UpdateAsync(
        Guid id,
        UpdateOrganizationDto input,
        CancellationToken cancellationToken = default);

    // Kimligi verilen organizasyonu alt agaciyla birlikte siler.
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Kullaniciyi organizasyona uye yapar.
    Task AddMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    // Kullaniciyi organizasyondan cikarir.
    Task RemoveMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
