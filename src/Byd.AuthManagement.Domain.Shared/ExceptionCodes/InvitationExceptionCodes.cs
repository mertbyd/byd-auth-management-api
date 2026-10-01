namespace Byd.AuthManagement.ExceptionCodes;

/// <summary>
/// Kullanici daveti akisinin hata kodlarini tanimlar.
/// </summary>
// islevi: Rol uygunlugu, rol-organizasyon baglantisi ve girdi dogrulamasi hatalarinin kararli kodlarini tutar.
// sistemdeki gorevi: Davet zincirinin kullandigi kodlarin tek kaynagidir.
public static class InvitationExceptionCodes
{
    // Rol yok ya da davet edenin kapsaminda degil; tenant'taki davet eden sadece kendi tenant'inin rolunu verir.
    public const string RoleNotAvailable = AuthManagementErrorCodes.InvitationGroup + "0001";

    // Rol birden fazla organizasyona bagli; kullanicinin hangisine uye yapilacagi belirsiz.
    public const string RoleInMultipleOrganizations = AuthManagementErrorCodes.InvitationGroup + "0002";

    // En az bir rol verilmeli.
    public const string RoleIdsRequired = AuthManagementErrorCodes.InvitationGroup + "0003";

    // Kullanici adi bos gecilemez.
    public const string UserNameRequired = AuthManagementErrorCodes.InvitationGroup + "0004";

    // E-posta bos gecilemez.
    public const string EmailRequired = AuthManagementErrorCodes.InvitationGroup + "0005";

    // E-posta gecerli bir adres olmali.
    public const string EmailInvalid = AuthManagementErrorCodes.InvitationGroup + "0006";

    // Baglantinin acilacagi istemci uygulamasi bos gecilemez.
    public const string AppNameRequired = AuthManagementErrorCodes.InvitationGroup + "0007";
}
