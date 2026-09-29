namespace Byd.AuthManagement.Permissions;

/// <summary>
/// Kullanici yonetiminin yetki sabitlerini tanimlar.
/// </summary>
// islevi: Kullanici yonetim yetkilerinin string sabitlerini tanimlar.
// sistemdeki gorevi: Controller action'lari yetki string'ini buradan okur; inline string kullanilmaz.
public partial class AuthManagementPermissions
{
    public static class Users
    {
        public const string Default = GroupName + ".Users";

        // Kullanici davet etme ve kullaniciya rol verme yetkisi.
        public const string Manage = Default + ".Manage";
    }
}
