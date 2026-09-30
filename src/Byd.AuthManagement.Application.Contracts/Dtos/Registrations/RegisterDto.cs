namespace Byd.AuthManagement.Dtos.Registrations;

/// <summary>
/// Yeni kullanici kayit istegini tanimlar.
/// </summary>
// islevi: Yeni kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Kayit endpointinin public girdi tipidir; sistemdeki aktif tenant ve rolleri otomatik baglanir.
public sealed class RegisterDto
{
    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}