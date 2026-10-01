namespace Byd.AuthManagement.Consts;

/// <summary>
/// Sifre sifirlama baglantisinin sabit degerlerini tanimlar.
/// </summary>
// islevi: Sifirlama baglantisinin acildigi istemci uygulamasinin adini ve sayfa yolunu tek yerde tutar.
// sistemdeki gorevi: Host baglanti adresini bu adla kaydeder, istemci istekte ayni adi gonderir.
public static class PasswordConsts
{
    // Sifirlama baglantisinin acildigi istemci uygulamasinin adi.
    public const string ClientAppName = "Web";

    // Istemcide sifre sifirlama sayfasinin yolu.
    public const string ResetPasswordPagePath = "account/reset-password";
}
