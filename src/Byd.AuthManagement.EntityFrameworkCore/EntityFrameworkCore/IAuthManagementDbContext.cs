using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.AuthManagement.EntityFrameworkCore;

[ConnectionStringName(AuthManagementDbProperties.ConnectionStringName)]
public interface IAuthManagementDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
