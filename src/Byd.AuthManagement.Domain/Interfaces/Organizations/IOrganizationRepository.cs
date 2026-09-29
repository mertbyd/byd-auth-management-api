using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Models.Organizations;
using Volo.Abp.Identity;

namespace Byd.AuthManagement.Interfaces.Organizations;

/// <summary>
/// Cercevenin organizasyon repository sozlesmesini ust adli okumalarla genisletir.
/// </summary>
// islevi: CRUD ve agac gezinmeyi cercevenin sozlesmesinden alir; okumalari ust kaydin adiyla birlikte dondurur.
// sistemdeki gorevi: Organizasyon kullanim senaryolari ile EF Core uygulamasi arasindaki veri erisim siniridir.
public interface IOrganizationRepository : IOrganizationUnitRepository
{
    // Kimligi verilen organizasyonu getirir; yoksa NotFound firlatir.
    Task<OrganizationModel> GetModelAsync(Guid id, CancellationToken cancellationToken = default);

    // Ust kaydin dogrudan alt kayitlarini getirir; ust kayit bos ise kokleri getirir.
    Task<List<OrganizationModel>> GetChildModelListAsync(Guid? parentId, CancellationToken cancellationToken = default);

    // Kapsamdaki tum organizasyonlari agac kodu sirasinda getirir.
    Task<List<OrganizationModel>> GetModelListAsync(CancellationToken cancellationToken = default);

    // Kullanicinin uyesi oldugu organizasyonlari getirir.
    Task<List<OrganizationModel>> GetUserModelListAsync(Guid userId, CancellationToken cancellationToken = default);

    // Rollerden birine bagli organizasyonlari rol baglariyla birlikte getirir.
    Task<List<OrganizationUnit>> GetListByRoleIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default);
}
