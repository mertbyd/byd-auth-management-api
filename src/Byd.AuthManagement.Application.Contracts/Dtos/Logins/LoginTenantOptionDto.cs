using System;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Kullanicinin girebilecegi bir tenant'i tasir.
/// </summary>
// islevi: Tenant kimligini, adini ve kullanicinin orada dogrudan rolu olup olmadigini HTTP ciktisina acar.
// sistemdeki gorevi: Login seceneklerinin tenant satiridir; kimligi ve adi bos satir host'tur.
public class LoginTenantOptionDto
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public bool HasRole { get; set; }
}
