using System;

namespace Byd.AuthManagement.Dtos.Registrations;

/// <summary>
/// Yeni kullanici kayit istegini tanimlar.
/// </summary>
// islevi: Kaydolunacak tenant'i ve yeni kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Kayit endpointinin public girdi tipidir; organizasyon kayitta secilmez.
public sealed class RegisterDto
{
    // Kaydolunacak tenant.
    public Guid TenantId { get; set; }

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}
