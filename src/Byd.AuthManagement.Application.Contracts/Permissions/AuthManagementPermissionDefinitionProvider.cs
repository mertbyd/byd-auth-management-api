using Byd.AuthManagement.Localization;
using Byd.AuthManagement.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Byd.AuthManagement.Permissions;

/// <summary>
/// Modulun yetki agacini ABP'ye tanimlar.
/// </summary>
// islevi: Yetki grubunu acar ve her ozellik ailesinin kendi partial metodunu cagirir.
// sistemdeki gorevi: Yeni bir ozellik ailesi eklemek bu dosyaya tek satir eklemekten ibarettir;
// mevcut satirlar degismez.
public partial class AuthManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            AuthManagementPermissions.GroupName,
            L(AuthManagementLocalizationKeys.Permissions.Group));

        AddOrganizationsPermissions(group);
    }

    // Localization anahtarini modul kaynagina baglar.
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuthManagementResource>(name);
    }
}
