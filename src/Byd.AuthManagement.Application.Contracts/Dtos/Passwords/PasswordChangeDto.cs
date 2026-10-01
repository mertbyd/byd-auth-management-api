namespace Byd.AuthManagement.Dtos.Passwords;

/// <summary>
/// Giris yapmis kullanicinin sifre degistirme istegini tanimlar.
/// </summary>
// islevi: Mevcut ve yeni sifreyi tasir.
// sistemdeki gorevi: Sifre degistirme endpointinin public girdi tipidir.
public sealed class PasswordChangeDto
{
    public string CurrentPassword { get; set; } = default!;

    public string NewPassword { get; set; } = default!;
}
