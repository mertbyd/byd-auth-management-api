using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Models.Organizations;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Byd.AuthManagement.EntityFrameworkCore.Repositories.Organizations;

/// <summary>
/// IOrganizationRepository sozlesmesini EF Core DbContext'e baglar.
/// </summary>
// islevi: Organizasyonu ust kaydina left join ile baglayip tek sorguda modele yansitir.
// sistemdeki gorevi: IOrganizationRepository sozlesmesinin provider uygulamasidir.
[ExposeServices(typeof(IOrganizationRepository))]
public class EfCoreOrganizationRepository(IDbContextProvider<IIdentityDbContext> dbContextProvider)
    : EfCoreOrganizationUnitRepository(dbContextProvider), IOrganizationRepository
{
    // Kimligi verilen organizasyonu ust kaydinin adiyla getirir; yoksa NotFound firlatir.
    public virtual async Task<OrganizationModel> GetModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = (await GetModelQueryableAsync(cancellationToken)).Where(organization => organization.Id == id);
        return await AsyncExecuter.FirstOrDefaultAsync(query, cancellationToken)
               ?? throw new EntityNotFoundException(typeof(OrganizationUnit), id);
    }

    // Ust kaydin dogrudan alt kayitlarini getirir; ust kayit bos ise kokleri getirir.
    public virtual async Task<List<OrganizationModel>> GetChildModelListAsync(
        Guid? parentId,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetModelQueryableAsync(cancellationToken)).Where(organization => organization.ParentId == parentId);
        return await AsyncExecuter.ToListAsync(query, cancellationToken);
    }

    // Kapsamdaki tum organizasyonlari agac kodu sirasinda getirir.
    public virtual async Task<List<OrganizationModel>> GetModelListAsync(CancellationToken cancellationToken = default)
    {
        var query = (await GetModelQueryableAsync(cancellationToken)).OrderBy(organization => organization.Code);
        return await AsyncExecuter.ToListAsync(query, cancellationToken);
    }

    // Kullanicinin uyelik kayitlarindan organizasyonlarini getirir.
    public virtual async Task<List<OrganizationModel>> GetUserModelListAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var memberships = dbContext.Set<IdentityUserOrganizationUnit>();
        var userRoles = dbContext.Set<IdentityUserRole>();
        var organizations = await GetQueryableAsync();
        var query = from membership in memberships
                    join organization in organizations
                        on membership.OrganizationUnitId equals organization.Id
                    where membership.UserId == userId
                    join parent in organizations
                        on organization.ParentId equals (Guid?)parent.Id into parents
                    from parent in parents.DefaultIfEmpty()
                    select new OrganizationModel
                    {
                        Id = organization.Id,
                        TenantId = organization.TenantId,
                        ParentId = organization.ParentId,
                        ParentName = parent == null ? null : parent.DisplayName,
                        Code = organization.Code,
                        DisplayName = organization.DisplayName,
                        HasRole = organization.Roles.Any(binding => userRoles.Any(userRole =>
                            userRole.UserId == userId && userRole.RoleId == binding.RoleId)),
                        CreationTime = organization.CreationTime,
                        LastModificationTime = organization.LastModificationTime
                    };
        return await AsyncExecuter.ToListAsync(query, cancellationToken);
    }

    // Rollerden birine bagli organizasyonlari rol baglariyla birlikte getirir.
    public virtual async Task<List<OrganizationUnit>> GetListByRoleIdsAsync(
        List<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetQueryableAsync())
            .IncludeDetails()
            .Where(organization => organization.Roles.Any(binding => roleIds.Contains(binding.RoleId)));
        return await AsyncExecuter.ToListAsync(query, cancellationToken);
    }

    // Organizasyonu ust kaydinin adiyla ve rol verip vermedigiyle modele yansitir.
    private async Task<IQueryable<OrganizationModel>> GetModelQueryableAsync(
        CancellationToken cancellationToken = default)
    {
        var organizations = await GetQueryableAsync();
        return from organization in organizations
               join parent in organizations on organization.ParentId equals (Guid?)parent.Id into parents
               from parent in parents.DefaultIfEmpty()
               select new OrganizationModel
               {
                   Id = organization.Id,
                   TenantId = organization.TenantId,
                   ParentId = organization.ParentId,
                   ParentName = parent == null ? null : parent.DisplayName,
                   Code = organization.Code,
                   DisplayName = organization.DisplayName,
                   HasRole = organization.Roles.Any(),
                   CreationTime = organization.CreationTime,
                   LastModificationTime = organization.LastModificationTime
               };
    }
}
