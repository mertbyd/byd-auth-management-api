namespace Byd.AuthManagement.Consts;

/// <summary>
/// Token sunucusunun modulce tanimlanan scope ve client adlarini tutar.
/// </summary>
// islevi: Seed edilen API scope'unun ve uygulama client'inin kararli adlarini tek yerde tutar.
// sistemdeki gorevi: Scope'u seed eden taraf ile token'i dogrulayan host ayni metni buradan okur.
public static class OpenIddictConsts
{
    // Modulun API scope'u; token'a audience olarak da yazilir.
    public const string ApiScope = "AuthManagement";

    // Sifre, refresh token ve token exchange akislariyla token alan public client.
    public const string AppClientId = "AuthManagement_App";

    // Client'in yonetim ekranlarinda gorunen adi.
    public const string AppClientDisplayName = "AuthManagement Application";
}
