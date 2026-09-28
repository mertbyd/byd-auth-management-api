using Microsoft.EntityFrameworkCore;
using Byd.AuthManagement.Extensions;
using Byd.CoreExtensions.EntityFrameworkCore;

namespace Byd.AuthManagement.EntityFrameworkCore;

/// <summary>
/// Auth DbContext'ini tasarim aninda ureten fabrika.
/// </summary>
// islevi: Context'in baglanti dizesi adini bildirir ve ornegini kurar; kalan her sey tabanda yasar.
// sistemdeki gorevi: Modulun migration setini uretirken dotnet ef'in kullandigi giris noktasidir.
public class AuthManagementDbContextFactory
    : CoreExtensionsDesignTimeDbContextFactory<AuthManagementDbContext>
{
    protected override string ConnectionStringName => AuthManagementDbProperties.ConnectionStringName;

    // Taban yalniz core'un semasini cozer; devralinan tablolarin semasi EF modulunun acilistaki
    // cagrilarinin karsiligi olarak burada cozulur.
    protected override AuthManagementDbContext CreateInstance(
        DbContextOptions<AuthManagementDbContext> options)
    {
        ServiceConfigurationContextExtension.ResolveSchemaAndPrefix();
        return new(options);
    }
}
