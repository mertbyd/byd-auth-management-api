namespace Byd.AuthManagement.ExceptionCodes;

/// <summary>
/// Organizasyonun girdi dogrulama hata kodlarini tanimlar.
/// </summary>
// islevi: Organizasyon adi icin zorunluluk ve uzunluk hatalarinin kararli kodlarini tutar.
// sistemdeki gorevi: Validator'larin kullandigi kodlarin tek kaynagidir.
public static class OrganizationExceptionCodes
{
    // Organizasyon adi bos gecilemez.
    public const string DisplayNameRequired = AuthManagementErrorCodes.OrganizationGroup + "0001";

    // Organizasyon adi cerceve kolonunun sinirini asamaz.
    public const string DisplayNameTooLong = AuthManagementErrorCodes.OrganizationGroup + "0002";

    // Organizasyon yoneticisinin kullanici adi bos gecilemez.
    public const string AdminUserNameRequired = AuthManagementErrorCodes.OrganizationGroup + "0003";

    // Organizasyon yoneticisinin e-postasi bos gecilemez.
    public const string AdminEmailRequired = AuthManagementErrorCodes.OrganizationGroup + "0004";

    // Organizasyon yoneticisinin e-postasi gecerli olmali.
    public const string AdminEmailInvalid = AuthManagementErrorCodes.OrganizationGroup + "0005";

    // Organizasyon yoneticisinin sifresi bos gecilemez.
    public const string AdminPasswordRequired = AuthManagementErrorCodes.OrganizationGroup + "0006";

    // Organizasyon islemi mevcut organizasyon kapsaminin disindadir.
    public const string OutsideScope = AuthManagementErrorCodes.OrganizationGroup + "0007";
}
