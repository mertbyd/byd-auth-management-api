namespace Byd.AuthManagement.Consts;

/// <summary>
/// Kullanici davetinin kararli sabitlerini tanimlar.
/// </summary>
// islevi: Davet e-postasinin email modulu veritabanindaki sablon adini tek yerde tutar.
// sistemdeki gorevi: Auth davet akisi ile merkezi email template satirini ayni ad uzerinden baglar.
public static class InvitationConsts
{
    // Davet e-postasinin email modulu template adi.
    public const string EmailTemplateName = "AuthManagement.UserInvitation";
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
