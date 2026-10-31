using Byd.AuthManagement.Consts;

namespace Byd.AuthManagement;

public static class AuthManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = "AuthManagement";

    public static string? DbSchema { get; set; } = DatabaseConsts.ModuleSchema;

    // Auth tablolarinin bagli oldugu baglanti dizesi; auth paketini kullanan modul host'u bu adla auth veritabanini, 
    // Default ile kendi veritabanini gosterir. Ad tanimli degilse ABP Default'a duser.
    public const string ConnectionStringName = "AuthManagement";
}
