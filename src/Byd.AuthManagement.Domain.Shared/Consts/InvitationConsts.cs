namespace Byd.AuthManagement.Consts;

/// <summary>
/// Kullanici davetinin sabit degerlerini tanimlar.
/// </summary>
// islevi: Davet e-postasinin sablon adini tek yerde tutar.
// sistemdeki gorevi: Sablon bu adla tanimlanir ve render edilir; email modulu ayni adla sablonu veritabanindan ezebilir.
public static class InvitationConsts
{
    // Davet e-postasi govdesinin sablon adi.
    public const string EmailTemplateName = "AuthManagement.UserInvitation";
}
