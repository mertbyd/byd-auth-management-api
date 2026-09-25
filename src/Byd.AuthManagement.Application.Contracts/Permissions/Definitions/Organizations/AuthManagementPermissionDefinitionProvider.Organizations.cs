using Byd.AuthManagement.Localization;
using Volo.Abp.Authorization.Permissions;

namespace Byd.AuthManagement.Permissions;

// islevi: Organizasyon yetkilerini ABP yetki agacina ekler.
// sistemdeki gorevi: Ana provider'in Define metodu bu partial metodu cagirir.
public partial class AuthManagementPermissionDefinitionProvider
{
    private void AddOrganizationsPermissions(PermissionGroupDefinition group)
    {
        var organizations = group.AddPermission(
            AuthManagementPermissions.Organizations.Default,
            L(AuthManagementLocalizationKeys.Permissions.Organizations));

        organizations.AddChild(
            AuthManagementPermissions.Organizations.View,
            L(AuthManagementLocalizationKeys.Permissions.OrganizationsView));

        organizations.AddChild(
            AuthManagementPermissions.Organizations.Manage,
            L(AuthManagementLocalizationKeys.Permissions.OrganizationsManage));
    }
}
