using System;

namespace Byd.AuthManagement.Models.Logins;

/// <summary>
/// Kullanicinin girebilecegi bir tenant'i tasir.
/// </summary>
// islevi: Tenant kimligini, adini ve kullanicinin orada dogrudan rolu olup olmadigini tasir.
// sistemdeki gorevi: Login listesinin tenant satiridir; kimligi bos satir host'tur.
public sealed class LoginTenantOptionModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public bool HasRole { get; set; }
}
