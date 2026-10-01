using System;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Kullanicinin uyesi oldugu bir organizasyonu tasir.
/// </summary>
// islevi: Organizasyonun kimligini, tenant'ini, agactaki yerini ve rol verip vermedigini HTTP ciktisina acar.
// sistemdeki gorevi: Login seceneklerinin organizasyon satiridir; istemci tenant'a gore gruplar.
public class LoginOrganizationOptionDto
{
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? ParentId { get; set; }

    public string? ParentName { get; set; }

    public string Code { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public bool HasRole { get; set; }
}
