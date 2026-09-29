using System;

namespace Byd.AuthManagement.Dtos.Registrations;

/// <summary>
/// Yeni kullanici kayit istegini tanimlar.
/// </summary>
// islevi: Kaydolunacak tenant'i, istege bagli organizasyonu ve yeni kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Kayit endpointinin public girdi tipidir.
public sealed class RegisterDto
{
    // Kaydolunacak tenant.
    public Guid TenantId { get; set; }

    // Katilinacak organizasyon; verilirse kullanici organizasyona uye olur ve organizasyonun varsayilan rollerini alir.
    public Guid? OrganizationId { get; set; }

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}
