namespace Byd.AuthManagement.ExceptionCodes;

/// <summary>
/// Sifre sifirlama ve degistirme akisinin hata kodlarini tanimlar.
/// </summary>
// islevi: Sifre isteklerinin girdi dogrulamasi hatalarinin kararli kodlarini tutar.
// sistemdeki gorevi: Sifre zincirinin kullandigi kodlarin tek kaynagidir; sifre kurallari cercevenin kodlariyla doner.
public static class PasswordExceptionCodes
{
    // E-posta bos gecilemez.
    public const string EmailRequired = AuthManagementErrorCodes.PasswordGroup + "0001";

    // E-posta gecerli bir adres olmali.
    public const string EmailInvalid = AuthManagementErrorCodes.PasswordGroup + "0002";

    // Sifirlama baglantisinin acilacagi uygulama adi bos gecilemez.
    public const string AppNameRequired = AuthManagementErrorCodes.PasswordGroup + "0003";

    // Kullanici bos gecilemez.
    public const string UserIdRequired = AuthManagementErrorCodes.PasswordGroup + "0004";

    // Sifirlama token'i bos gecilemez.
    public const string ResetTokenRequired = AuthManagementErrorCodes.PasswordGroup + "0005";

    // Yeni sifre bos gecilemez.
    public const string PasswordRequired = AuthManagementErrorCodes.PasswordGroup + "0006";

    // Mevcut sifre bos gecilemez.
    public const string CurrentPasswordRequired = AuthManagementErrorCodes.PasswordGroup + "0007";

    // Yeni sifre mevcut sifreyle ayni olamaz.
    public const string NewPasswordSameAsCurrent = AuthManagementErrorCodes.PasswordGroup + "0008";
}
