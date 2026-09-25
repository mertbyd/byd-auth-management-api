using Byd.AuthManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Byd.AuthManagement.Permissions;

public class AuthManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AuthManagementPermissions.GroupName, L("Permission:AuthManagement"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuthManagementResource>(name);
    }
}
