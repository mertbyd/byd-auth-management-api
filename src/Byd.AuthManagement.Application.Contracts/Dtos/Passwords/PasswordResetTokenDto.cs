using System;

namespace Byd.AuthManagement.Dtos.Passwords;

/// <summary>
/// Sifre sifirlama token'i dogrulama istegini tanimlar.
/// </summary>
// islevi: Sifirlama baglantisindaki kullaniciyi ve token'i tasir.
// sistemdeki gorevi: Token dogrulama endpointinin public girdi tipidir.
public sealed class PasswordResetTokenDto
{
    public Guid UserId { get; set; }

    public string ResetToken { get; set; } = default!;
}
