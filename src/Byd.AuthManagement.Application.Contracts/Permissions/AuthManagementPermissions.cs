using Volo.Abp.Reflection;

namespace Byd.AuthManagement.Permissions;

public class AuthManagementPermissions
{
    public const string GroupName = "AuthManagement";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AuthManagementPermissions));
    }
}
