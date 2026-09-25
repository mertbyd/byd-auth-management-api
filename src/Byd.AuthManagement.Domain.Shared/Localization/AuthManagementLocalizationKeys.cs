namespace Byd.AuthManagement.Localization;

// islevi: Modul localization kaynaklarinda kod tarafindan kullanilan kararli anahtarlari tanimlar.
// sistemdeki gorevi: Izin tanimlarinin localization JSON anahtarlariyla compile-time tek sozlesmede
// bulusmasini saglar; provider'lar inline string tasimaz.
public static class AuthManagementLocalizationKeys
{
    public static class Permissions
    {
        public const string Group = "Permission:AuthManagement";
        public const string Organizations = "Permission:Organizations";
        public const string OrganizationsView = "Permission:Organizations.View";
        public const string OrganizationsManage = "Permission:Organizations.Manage";
    }
}
