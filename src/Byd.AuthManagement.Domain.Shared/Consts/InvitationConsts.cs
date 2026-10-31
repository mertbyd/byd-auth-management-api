namespace Byd.AuthManagement.Consts;

/// <summary>
/// Kullanici davetinin kararli sabitlerini tanimlar.
/// </summary>
// islevi: Davet baglantisinin parcalarini ve davet e-postasinin varsayilan sablon kaynaklarini tek yerde tutar.
// sistemdeki gorevi: Auth davet akisi, baglanti kurucusu ve varsayilan sablon seed'i ayni degerleri buradan okur.
public static class InvitationConsts
{
    // Davet e-postasinin core olay tablosundaki tekil adi; seed bu adla olayi acar, gonderim bu adla sablonu bulur.
    public const string EmailEventName = "AuthManagement.UserInvitation";

    // Davet e-postasinin varsayilan sablonunun gomulu dosya yolu; {0} sablonun dilidir.
    public const string EmailTemplateVirtualPathFormat = "/Invitations/Templates/UserInvitation/{0}.tpl";

    // Varsayilan davet sablonu gomulu olan diller; seed her dil icin host kapsaminda bir sablon acar.
    public static readonly string[] EmailTemplateCultureNames = ["tr", "en"];

    public const string AuthorizationCallbackPath = "/connect/authorize/callback";
    public const string AuthorizationPath = "/connect/authorize?";
    public const string UserIdQueryParameterName = "userId";
    public const string ResetTokenQueryParameterName = "resetToken";
    public const string ReturnUrlQueryParameterName = "returnUrl";
    public const string ReturnUrlHashQueryParameterName = "returnUrlHash";
    public const string RedirectUriQueryParameterName = "redirect_uri";
    public const char QueryStartCharacter = '?';
    public const char QueryParameterSeparatorCharacter = '&';
    public const char QueryValueSeparatorCharacter = '=';
}
