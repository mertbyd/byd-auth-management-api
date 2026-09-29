namespace Byd.AuthManagement.Consts;

/// <summary>
/// Hesap HTTP rotalarini tanimlar.
/// </summary>
// islevi: Login, kayit ve sifre endpointlerinin kararli rota sozlesmesini tek yerde tutar.
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

    // Yeni kullanici kayit endpointinin rotasidir.
    public const string Register = "register";

    // Sifre sifirlama baglantisini e-postaya gonderen endpointin rotasidir.
    public const string SendPasswordResetCode = "send-password-reset-code";

    // Sifre sifirlama token'ini dogrulayan endpointin rotasidir.
    public const string VerifyPasswordResetToken = "verify-password-reset-token";

    // Token ile yeni sifre belirleyen endpointin rotasidir.
    public const string ResetPassword = "reset-password";

    // Giris yapmis kullanicinin sifresini degistiren endpointin rotasidir.
    public const string ChangePassword = "change-password";
}
