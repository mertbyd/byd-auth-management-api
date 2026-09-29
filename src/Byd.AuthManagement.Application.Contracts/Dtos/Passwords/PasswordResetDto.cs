using System;

namespace Byd.AuthManagement.Dtos.Passwords;

/// <summary>
/// Token'la yeni sifre belirleme istegini tanimlar.
/// </summary>
// islevi: Sifirlama baglantisindaki kullaniciyi, token'i ve yeni sifreyi tasir.
// sistemdeki gorevi: Sifre sifirlama endpointinin public girdi tipidir.
public sealed class PasswordResetDto
{
    public Guid UserId { get; set; }

    public string ResetToken { get; set; } = default!;

    public string Password { get; set; } = default!;
}
