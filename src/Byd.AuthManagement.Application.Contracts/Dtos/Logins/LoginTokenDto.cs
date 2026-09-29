namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Token sunucusunun verdigi token'lari tasir.
/// </summary>
// islevi: Erisim token'ini, tipini, refresh token'i ve gecerlilik suresini tasir.
// sistemdeki gorevi: Login ve refresh endpointlerinin ortak token cevabidir.
public class LoginTokenDto
{
    public string AccessToken { get; set; } = default!;

    public string TokenType { get; set; } = default!;

    public string? RefreshToken { get; set; }

    // Erisim token'inin saniye cinsinden gecerlilik suresi.
    public int ExpiresIn { get; set; }
}
