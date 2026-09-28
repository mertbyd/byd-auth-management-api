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
}
