using System;
using Byd.CoreExtensions.Scopes;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Oturumdaki bir rolu tasir.
/// </summary>
// islevi: Rolun kimligini ve adini HTTP ciktisina acar.
// sistemdeki gorevi: Oturum bilgisi cevabinin rol satiridir.
public class CurrentLoginRoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public ScopeLevel ScopeLevel { get; set; }
}
