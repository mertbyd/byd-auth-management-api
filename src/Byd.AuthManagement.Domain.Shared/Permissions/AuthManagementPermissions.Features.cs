namespace Byd.AuthManagement.Permissions;

/// <summary>
/// Organizasyon feature yonetiminin yetki sabitlerini tanimlar.
/// </summary>
// islevi: Organizasyon feature degerlerini ABP Feature Management API'si uzerinden yonetme yetkisini tanimlar.
// sistemdeki gorevi: O provider policy'sinin inline string kullanilmayan authorization sozlesmesidir.
public partial class AuthManagementPermissions
{
    public static class Features
    {
        public const string Default = GroupName + ".Features";
        public const string Manage = Default + ".Manage";
    }
}
