using Byd.AuthManagement.Localization;
using Volo.Abp.Authorization.Permissions;

namespace Byd.AuthManagement.Permissions;

// islevi: Kullanici yonetim yetkilerini ABP yetki agacina ekler.
// sistemdeki gorevi: Ana provider'in Define metodu bu partial metodu cagirir.
public partial class AuthManagementPermissionDefinitionProvider
{
    private void AddUsersPermissions(PermissionGroupDefinition group)
    {
        var users = group.AddPermission(
            AuthManagementPermissions.Users.Default,
            L(AuthManagementLocalizationKeys.Permissions.Users));

        users.AddChild(
            AuthManagementPermissions.Users.Manage,
            L(AuthManagementLocalizationKeys.Permissions.UsersManage));
    }
}
