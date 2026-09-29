using System;

namespace Byd.AuthManagement.Models.Registrations;

/// <summary>
/// Kayit isteginin domain girdisini tasir.
/// </summary>
// islevi: Kaydolunacak tenant'i ve yeni kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Mapperly ile AccountManager kaydi arasindaki girdi sozlesmesidir.
public sealed class RegistrationModel
{
    public Guid TenantId { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
