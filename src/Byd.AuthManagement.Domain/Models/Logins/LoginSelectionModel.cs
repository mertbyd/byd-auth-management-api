using System;
using System.Collections.Generic;
using Byd.CoreExtensions.Scopes;
using Volo.Abp.Identity;

namespace Byd.AuthManagement.Models.Logins;

/// <summary>
/// Dogrulanmis kapsam secimini ve o kapsamdaki rolleri tasir.
/// </summary>
// islevi: Secilen tenant, organizasyon ve organizasyonun ust organizasyonunu adlariyla, kapsamdaki rollerle birlikte tasir;
// secim eksikse bos kapsamdir.
// sistemdeki gorevi: Token istegine eklenen secimin ve token'a yazilan claim'lerin tek kaynagidir.
public sealed class LoginSelectionModel
{
    public Guid? TenantId { get; set; }
    public string? TenantName { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Guid? ParentOrganizationId { get; set; }
    public string? ParentOrganizationName { get; set; }
    public ScopeLevel ScopeLevel { get; set; }
    // Secim eksik; token kapsamsiz ve rolsuz uretilir, kapsam switch ile secilir.
    public bool IsSelectionRequired { get; set; }
    public List<IdentityRole> Roles { get; set; } = [];
}
