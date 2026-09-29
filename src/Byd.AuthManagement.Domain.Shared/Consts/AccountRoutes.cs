namespace Byd.AuthManagement.Consts;

/// <summary>
/// Hesap HTTP rotalarini tanimlar.
/// </summary>
// islevi: Login ve oturum endpointlerinin kararli rota sozlesmesini tek yerde tutar.
// sistemdeki gorevi: Controller ile istemcilerin ayni derleme guvenli route degerlerini kullanmasini saglar.
public static class AccountRoutes
{
    // Hesap endpoint ailesinin modul kok rotasidir.
    public const string Root = "api/auth-management/account";

    // Kullanici adi ve sifreyle giris endpointinin rotasidir.
    public const string Login = "login";

    // Oturumdaki token'la tenant ve organizasyon secen ya da degistiren endpointin rotasidir.
    public const string Switch = "switch";

    // Refresh token ile yeni token alan endpointin rotasidir.
    public const string Refresh = "refresh";

    // Oturum kullanicisinin kimlik ve kapsam bilgilerini donduren endpointin rotasidir.
    public const string Me = "me";

    // Oturumu kapatan endpointin rotasidir.
    public const string Logout = "logout";
}
