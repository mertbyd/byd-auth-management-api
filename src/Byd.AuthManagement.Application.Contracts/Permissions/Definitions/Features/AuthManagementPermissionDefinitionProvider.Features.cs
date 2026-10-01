using Byd.AuthManagement.Localization;
using Volo.Abp.Authorization.Permissions;

namespace Byd.AuthManagement.Permissions;

// islevi: Feature yetkilerini ABP yetki agacina ekler.
// sistemdeki gorevi: Ana provider'in Define metodu bu partial metodu cagirir.
public partial class AuthManagementPermissionDefinitionProvider
{
    private void AddFeaturesPermissions(PermissionGroupDefinition group)
    {
        var features = group.AddPermission(
            AuthManagementPermissions.Features.Default,
            L(AuthManagementLocalizationKeys.Permissions.Features));

        features.AddChild(
            AuthManagementPermissions.Features.Manage,
            L(AuthManagementLocalizationKeys.Permissions.FeaturesManage));
    }
}
