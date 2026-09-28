namespace Byd.AuthManagement.Permissions;

/// <summary>
/// Organizasyonun yetki sabitlerini tanimlar.
/// </summary>
// islevi: Organizasyon okuma ve yonetim yetkilerinin string sabitlerini tanimlar.
// sistemdeki gorevi: Controller action'lari yetki string'ini buradan okur; inline string kullanilmaz.
public partial class AuthManagementPermissions
{
    public static class Organizations
    {
        public const string Default = GroupName + ".Organizations";

        // Organizasyon agacini ve detayini okuma yetkisi.
        public const string View = Default + ".View";

        // Organizasyon olusturma, guncelleme, tasima ve silme yetkisi.
        public const string Manage = Default + ".Manage";
    }
}
