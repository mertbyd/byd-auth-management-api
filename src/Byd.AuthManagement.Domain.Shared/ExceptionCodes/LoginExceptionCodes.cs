namespace Byd.AuthManagement.ExceptionCodes;

/// <summary>
/// Login akisinin hata kodlarini tanimlar.
/// </summary>
// islevi: Kimlik bilgisi, kapsam secimi, token istegi ve girdi dogrulamasi hatalarinin kararli kodlarini tutar.
// sistemdeki gorevi: Login zincirinin kullandigi kodlarin tek kaynagidir.
public static class LoginExceptionCodes
{
    // Kullanici adi veya sifre hatali; hangisinin hatali oldugu soylenmez.
    public const string InvalidCredentials = AuthManagementErrorCodes.LoginGroup + "0001";

    // Hatali denemeler yuzunden hesap gecici olarak kilitli.
    public const string LockedOut = AuthManagementErrorCodes.LoginGroup + "0002";

    // Hesap pasif ya da dogrulanmamis; giris izni yok.
    public const string NotAllowed = AuthManagementErrorCodes.LoginGroup + "0003";

    // Secilen tenant ya da organizasyon kullanicinin kapsamlari arasinda degil.
    public const string ScopeNotAvailable = AuthManagementErrorCodes.LoginGroup + "0004";

    // Secilen kapsamda kullanicinin hicbir rolu yok.
    public const string NoRoleInScope = AuthManagementErrorCodes.LoginGroup + "0005";

    // Token sunucusu istegi reddetti ya da token ucunun adresi okunamadi.
    public const string TokenRequestFailed = AuthManagementErrorCodes.LoginGroup + "0006";

    // Kullanici adi bos gecilemez.
    public const string UserNameRequired = AuthManagementErrorCodes.LoginGroup + "0007";

    // Sifre bos gecilemez.
    public const string PasswordRequired = AuthManagementErrorCodes.LoginGroup + "0008";

    // Refresh token bos gecilemez.
    public const string RefreshTokenRequired = AuthManagementErrorCodes.LoginGroup + "0009";

    // Gecilecek tenant bos gecilemez.
    public const string TenantIdRequired = AuthManagementErrorCodes.LoginGroup + "0010";
}
