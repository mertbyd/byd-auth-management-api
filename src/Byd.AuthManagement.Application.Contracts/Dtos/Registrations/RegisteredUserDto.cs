using System;

namespace Byd.AuthManagement.Dtos.Registrations;

/// <summary>
/// Kaydedilen kullaniciyi ve kaydoldugu tenant'i tasir.
/// </summary>
// islevi: Kayit sonucunun kullanici, tenant ve varsa organizasyon bilgisini HTTP ciktisina acar.
// sistemdeki gorevi: Kayit endpointinin cevabidir.
public class RegisteredUserDto
{
    public Guid TenantId { get; set; }

    public string TenantName { get; set; } = default!;

    public Guid? OrganizationId { get; set; }

    public string? OrganizationName { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;
}
