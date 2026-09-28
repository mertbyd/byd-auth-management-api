using Byd.AuthManagement.Consts;

namespace Byd.AuthManagement;

public static class AuthManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = "AuthManagement";

    public static string? DbSchema { get; set; } = DatabaseConsts.ModuleSchema;

    public const string ConnectionStringName = "Default";
}
