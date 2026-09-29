using System;
using System.Collections.Generic;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Istegi yapan kullanicinin kimligini, kapsamini, rollerini ve uyeliklerini tasir.
/// </summary>
// islevi: Kullaniciyi, token'daki kapsami ve rolleri, oturumdaki tenant'taki uyeliklerle HTTP ciktisina acar.
// sistemdeki gorevi: Oturum bilgisi endpointinin cevabidir.
public class CurrentLoginDto
{
    public Guid? TenantId { get; set; }

    public string? TenantName { get; set; }

    public Guid? OrganizationId { get; set; }

    public string? OrganizationName { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = default!;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public List<CurrentLoginRoleDto> Roles { get; set; } = [];

    public List<CurrentLoginOrganizationDto> Organizations { get; set; } = [];
}
