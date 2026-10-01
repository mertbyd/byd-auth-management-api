namespace Byd.AuthManagement.Consts;

/// <summary>
/// Super admin rolunun ve istege bagli baslangic hesabinin sabitlerini tanimlar.
/// </summary>
// islevi: Host kapsamindaki yonetim rolunun adini ve seed ayarlarinin config anahtarlarini tek yerde tutar.
// sistemdeki gorevi: Seed edilen rol, login kapsaminda SuperAdmin seviyesini tasir ve ABP host yonetim uclarini acar.
public static class SuperAdminConsts
{
    public const string RoleName = "SuperAdmin";

    public const string UserNameConfigurationKey = "AuthServer:SuperAdminUserName";

    public const string EmailConfigurationKey = "AuthServer:SuperAdminEmail";

    public const string PasswordConfigurationKey = "AuthServer:SuperAdminPassword";
}
