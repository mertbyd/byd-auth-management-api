using System;
using Byd.CoreExtensions.Scopes;

namespace Byd.AuthManagement.Models.Logins;

/// <summary>
/// Oturumdaki rolü ve mevcut veriden türetilen kapsam seviyesini taşır.
/// </summary>
// islevi: Rol kimligi ve adini, organizasyon bagindan turetilen kapsam seviyesiyle birlestirir.
// sistemdeki gorevi: Me cevabindaki rol satirinin domain modelidir.
public sealed class CurrentLoginRoleModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public ScopeLevel ScopeLevel { get; set; }
}
