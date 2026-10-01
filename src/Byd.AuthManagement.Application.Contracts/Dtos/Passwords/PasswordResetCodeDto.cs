namespace Byd.AuthManagement.Dtos.Passwords;

/// <summary>
/// Sifre sifirlama baglantisi istegini tanimlar.
/// </summary>
// islevi: Baglantinin gonderilecegi e-postayi ve baglantinin acilacagi istemci uygulamasini tasir.
// sistemdeki gorevi: Sifirlama baglantisi endpointinin public girdi tipidir.
public sealed class PasswordResetCodeDto
{
    public string Email { get; set; } = default!;

    // Baglantinin acilacagi istemci uygulamasinin adi; host bu adla adres tanimlar.
    public string AppName { get; set; } = default!;

    // Sifre belirlendikten sonra istemcinin donecegi adres.
    public string? ReturnUrl { get; set; }

    public string? ReturnUrlHash { get; set; }
}
