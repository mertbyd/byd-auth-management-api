using System.Collections.Generic;
using Byd.AuthManagement.Models.Organizations;

namespace Byd.AuthManagement.Models.Logins;

/// <summary>
/// Kullanicinin girebilecegi tenant ve organizasyonlari tasir.
/// </summary>
// islevi: Tenant listesini ve uyelik listesini birlikte tasir.
// sistemdeki gorevi: Secim gerekip gerekmedigine bu modelle karar verilir; istemciye secenek olarak doner.
public sealed class LoginScopeModel
{
    public List<LoginTenantOptionModel> Tenants { get; set; } = [];
    public List<OrganizationModel> Organizations { get; set; } = [];
}
