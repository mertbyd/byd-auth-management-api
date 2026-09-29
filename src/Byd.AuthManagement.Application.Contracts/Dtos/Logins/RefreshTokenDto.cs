namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Refresh token ile yeni token istegini tanimlar.
/// </summary>
// islevi: Onceki login'de verilen refresh token'i tasir.
// sistemdeki gorevi: Refresh endpointinin public girdi tipidir; kapsam refresh token'in icinde tasinir.
public sealed class RefreshTokenDto
{
    public string RefreshToken { get; set; } = default!;
}
