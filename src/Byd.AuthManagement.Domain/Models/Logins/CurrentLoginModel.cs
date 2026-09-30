using System;
using System.Collections.Generic;
using Byd.AuthManagement.Models.Organizations;
using Byd.CoreExtensions.Scopes;

namespace Byd.AuthManagement.Models.Logins;

/// <summary>
/// Istegi yapan kullanicinin kimligini, kapsamini, rollerini ve uyeliklerini tasir.
/// </summary>
// islevi: Kullaniciyi, token'daki kapsami ve rolleri, oturumdaki tenant'taki uyeliklerle birlikte tasir.
// sistemdeki gorevi: Oturum bilgisi endpointinin kaynagidir; yetki kontrolunun gordugu kapsami gosterir.
public sealed class CurrentLoginModel
{
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public ScopeLevel ScopeLevel { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public List<CurrentLoginRoleModel> Roles { get; set; } = [];
    public List<OrganizationModel> Organizations { get; set; } = [];
}
