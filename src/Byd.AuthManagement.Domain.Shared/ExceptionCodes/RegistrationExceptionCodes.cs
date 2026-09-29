namespace Byd.AuthManagement.ExceptionCodes;

/// <summary>
/// Kayit akisinin hata kodlarini tanimlar.
/// </summary>
// islevi: Tenant uygunlugu, varsayilan rol ve girdi dogrulamasi hatalarinin kararli kodlarini tutar.
// sistemdeki gorevi: Kayit zincirinin kullandigi kodlarin tek kaynagidir.
public static class RegistrationExceptionCodes
{
    // Secilen tenant yok ya da pasif.
    public const string TenantNotAvailable = AuthManagementErrorCodes.RegistrationGroup + "0001";

    // Tenant'ta varsayilan rol yok; tenant kayda kapali.
    public const string NoDefaultRole = AuthManagementErrorCodes.RegistrationGroup + "0002";

    // Tenant bos gecilemez.
    public const string TenantIdRequired = AuthManagementErrorCodes.RegistrationGroup + "0003";

    // Kullanici adi bos gecilemez.
    public const string UserNameRequired = AuthManagementErrorCodes.RegistrationGroup + "0004";

    // E-posta bos gecilemez.
    public const string EmailRequired = AuthManagementErrorCodes.RegistrationGroup + "0005";

    // E-posta gecerli bir adres olmali.
    public const string EmailInvalid = AuthManagementErrorCodes.RegistrationGroup + "0006";

    // Sifre bos gecilemez.
    public const string PasswordRequired = AuthManagementErrorCodes.RegistrationGroup + "0007";

    // Tenant acan kayitta tenant adi bos gecilemez.
    public const string TenantNameRequired = AuthManagementErrorCodes.RegistrationGroup + "0008";

    // Tenant adi cerceve kolonunun sinirini asamaz.
    public const string TenantNameTooLong = AuthManagementErrorCodes.RegistrationGroup + "0009";
}
