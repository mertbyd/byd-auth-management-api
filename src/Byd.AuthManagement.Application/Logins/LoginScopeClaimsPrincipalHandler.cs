using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Managers.Accounts;
using Byd.AuthManagement.Models.Logins;
using Byd.CoreExtensions.Consts;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Security.Claims;

namespace Byd.AuthManagement.Logins;

/// <summary>
/// Token uretilirken secilen kapsami claim'lere yazar ve rolleri o kapsama indirger.
/// </summary>
// islevi: Kapsami sifre ve token exchange isteginde form parametresinden, refresh isteginde token'dan okur ve Manager'a dogrulatir.
// sistemdeki gorevi: Token'daki kapsamin tek yazaridir; secim eksikse token kapsamsiz ve rolsuz kalir.
public class LoginScopeClaimsPrincipalHandler(AccountManager accountManager)
    : IAbpOpenIddictClaimsPrincipalHandler, ITransientDependency
{
    // Yalniz sifre, refresh ve token exchange isteklerinde calisir; diger akislarin principal'ina dokunmaz.
    public virtual async Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context)
    {
        var request = context.OpenIddictRequest;
        if (!request.IsPasswordGrantType() && !request.IsRefreshTokenGrantType() && !request.IsTokenExchangeGrantType())
            return;
        var userId = context.Principal.FindUserId()!.Value;
        var selection = await ResolveSelectionAsync(context, userId);
        var identity = context.Principal.Identities.First();
        SetRoles(identity, selection.Roles);
        SetClaim(identity, TenantClaimTypes.TenantId, selection.TenantId?.ToString());
        SetClaim(identity, TenantClaimTypes.TenantName, selection.TenantName);
        SetClaim(identity, OrganizationClaimTypes.OrganizationId, selection.OrganizationId?.ToString());
        SetClaim(identity, OrganizationClaimTypes.OrganizationName, selection.OrganizationName);
        SetClaim(identity, OrganizationClaimTypes.ParentId, selection.ParentOrganizationId?.ToString());
        SetClaim(identity, OrganizationClaimTypes.ParentName, selection.ParentOrganizationName);
        SetClaim(identity, ScopeClaimTypes.Level, selection.IsSelectionRequired ? null : selection.ScopeLevel.ToString());
    }

    // Switch ve secilmis refresh kapsamini aynen yeniden dogrular; ilk login tek secenegi tamamlayabilir.
    private async Task<LoginSelectionModel> ResolveSelectionAsync(
        AbpOpenIddictClaimsPrincipalHandlerContext context,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var tenantId = ReadScopeId(context, OpenIddictConsts.TenantParameter, TenantClaimTypes.TenantId);
        var organizationId = ReadScopeId(context, OpenIddictConsts.OrganizationParameter, OrganizationClaimTypes.OrganizationId);
        if (context.OpenIddictRequest.IsTokenExchangeGrantType() ||
            context.OpenIddictRequest.IsRefreshTokenGrantType() &&
            context.Principal.HasClaim(claim => claim.Type == ScopeClaimTypes.Level))
            return await accountManager.ResolveSelectionAsync(userId, tenantId, organizationId, cancellationToken: cancellationToken);

        return await accountManager.ResolveTokenSelectionAsync(
            userId,
            await accountManager.GetScopeAsync(userId, cancellationToken),
            tenantId,
            organizationId,
            cancellationToken);
    }

    // Refresh yalniz token kapsamini okur; switch'te bos alan eski token'dan doldurulmaz.
    private static Guid? ReadScopeId(
        AbpOpenIddictClaimsPrincipalHandlerContext context,
        string parameterName,
        string claimType)
    {
        var value = context.OpenIddictRequest.IsRefreshTokenGrantType()
            ? context.Principal.FindFirst(claimType)?.Value
            : (string?)context.OpenIddictRequest.GetParameter(parameterName);
        if (string.IsNullOrEmpty(value))
            return null;
        return Guid.TryParse(value, out var id) && id != Guid.Empty
            ? id : throw new BusinessException(LoginExceptionCodes.ScopeNotAvailable);
    }

    // Rol claim'lerini kapsamdaki rollerin adi ve kimligiyle degistirir.
    private static void SetRoles(ClaimsIdentity identity, List<IdentityRole> roles)
    {
        identity.RemoveAll(AbpClaimTypes.Role);
        identity.RemoveAll(RoleClaimTypes.RoleId);
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(AbpClaimTypes.Role, role.Name));
            identity.AddClaim(new Claim(RoleClaimTypes.RoleId, role.Id.ToString()));
        }
    }

    // Claim'i verilen degerle degistirir; deger bossa claim kaldirilir.
    private static void SetClaim(ClaimsIdentity identity, string claimType, string? value)
    {
        identity.RemoveAll(claimType);
        if (value is not null)
            identity.AddClaim(new Claim(claimType, value));
    }
}
