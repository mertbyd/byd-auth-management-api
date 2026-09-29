using System;
using System.Collections.Generic;

namespace Byd.AuthManagement.Models.Registrations;

/// <summary>
/// Kaydolan kullanicinin girecegi tenant'i, organizasyonu ve rolleri tasir.
/// </summary>
// islevi: Kayitta acilan ya da secilen tenant'i adiyla, varsa katilinacak organizasyonu ve dogrudan verilecek rol
// adlarini tasir.
// sistemdeki gorevi: Kayit yollarinin ortak ciktisidir; kalan adimlar bunu kullanir. Organizasyon rolleri uyelikle
// geldigi icin organizasyonlu kayitta rol adi tasinmaz.
public sealed class RegistrationScopeModel
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = default!;
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public List<string> RoleNames { get; set; } = [];
}
