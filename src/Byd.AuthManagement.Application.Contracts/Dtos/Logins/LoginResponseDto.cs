using System.Collections.Generic;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Login isteginin sonucunu tasir: kapsam secenekleri ve token.
/// </summary>
// islevi: Kullanicinin girebilecegi tenant ve organizasyonlari ve token'i tasir.
// sistemdeki gorevi: Login endpointinin cevabidir; secim gerekiyorsa token kapsamsizdir, istemci kapsami switch ile secer.
public class LoginResponseDto
{
    // Secim eksik; Token kapsamsiz ve rolsuzdur.
    public bool IsSelectionRequired { get; set; }

    public List<LoginTenantOptionDto> Tenants { get; set; } = [];

    public List<LoginOrganizationOptionDto> Organizations { get; set; } = [];

    public LoginTokenDto Token { get; set; } = default!;
}
