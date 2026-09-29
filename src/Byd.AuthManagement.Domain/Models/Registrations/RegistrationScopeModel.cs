using System;
using System.Collections.Generic;

namespace Byd.AuthManagement.Models.Registrations;

/// <summary>
/// Kaydolan kullanicinin girecegi tenant'i ve rolleri tasir.
/// </summary>
// islevi: Kayitta acilan ya da secilen tenant'i adiyla, kullaniciya verilecek rol adlariyla birlikte tasir.
// sistemdeki gorevi: Tenant acan ve mevcut tenant'a katilan kayit yollarinin ortak ciktisidir; kalan adimlar bunu kullanir.
public sealed class RegistrationScopeModel
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = default!;
    public List<string> RoleNames { get; set; } = [];
}
