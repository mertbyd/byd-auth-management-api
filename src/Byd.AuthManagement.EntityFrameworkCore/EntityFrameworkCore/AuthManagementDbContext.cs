using Microsoft.EntityFrameworkCore;
using Byd.CoreExtensions.EntityFrameworkCore;
using Byd.CoreExtensions.Extensions;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Byd.AuthManagement.EntityFrameworkCore;

[ConnectionStringName(AuthManagementDbProperties.ConnectionStringName)]
public class AuthManagementDbContext : BydDbContext<AuthManagementDbContext>, IAuthManagementDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public AuthManagementDbContext(DbContextOptions<AuthManagementDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAuthManagement();

        builder.ApplySnakeCaseNaming();
    }
}
