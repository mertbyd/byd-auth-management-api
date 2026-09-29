using System;

namespace Byd.AuthManagement.Dtos.Registrations;

/// <summary>
/// Kaydedilen kullaniciyi ve kaydoldugu tenant'i tasir.
/// </summary>
// islevi: Kayit sonucunun kullanici ve tenant bilgisini HTTP ciktisina acar.
// sistemdeki gorevi: Kayit endpointinin cevabidir.
public class RegisteredUserDto
{
    public Guid TenantId { get; set; }

    public string TenantName { get; set; } = default!;

    public Guid UserId { get; set; }

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;
}
