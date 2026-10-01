namespace Byd.AuthManagement.Models.Registrations;

/// <summary>
/// Tenant acan kayit isteginin domain girdisini tasir.
/// </summary>
// islevi: Acilacak tenant'in adini ve yeni kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Mapperly ile AccountManager'in tenant acan kaydi arasindaki girdi sozlesmesidir.
public sealed class TenantRegistrationModel
{
    public string TenantName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
