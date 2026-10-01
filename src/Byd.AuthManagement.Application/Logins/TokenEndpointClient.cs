using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Models.Logins;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Byd.AuthManagement.Logins;

/// <summary>
/// Token sunucusunun token ucuna sifre, refresh token ve token exchange istegi gonderir.
/// </summary>
// islevi: Token isteklerini OAuth form istegine cevirir; reddedilen istegi is hatasina donusturur.
// sistemdeki gorevi: Token ucuyla konusan tek yerdir; ucun adresi sunucunun discovery dokumanindan okunur.
public class TokenEndpointClient(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    DiscoveryCache discoveryCache) : ITransientDependency
{
    // Istegin Authorization basligindaki erisim token'idir.
    private string CurrentAccessToken =>
        AuthenticationHeaderValue.Parse(httpContextAccessor.HttpContext!.Request.Headers.Authorization!).Parameter!;

    // Token ucunun adresini sunucunun yayinladigi dokumandan okur; dokuman bir kez cekilip saklanir.
    private async Task<string> GetTokenEndpointAsync(CancellationToken cancellationToken = default)
    {
        var discovery = await discoveryCache.GetAsync().WaitAsync(cancellationToken);
        if (discovery.IsError)
            throw new BusinessException(LoginExceptionCodes.TokenRequestFailed, details: discovery.Error);

        return discovery.TokenEndpoint!;
    }

    // Sifreyle ve secilen kapsamla token ister; kapsamin bos alani istege eklenmez.
    public virtual async Task<TokenResponse> RequestPasswordTokenAsync(
        string userName,
        string password,
        LoginSelectionModel selection,
        CancellationToken cancellationToken = default)
    {
        var request = new PasswordTokenRequest
        {
            Address = await GetTokenEndpointAsync(cancellationToken),
            ClientId = OpenIddictConsts.AppClientId,
            Scope = OpenIddictConsts.TokenScopes,
            UserName = userName,
            Password = password
        };
        AddSelection(request.Parameters, selection);
        var response = await httpClientFactory.CreateClient().RequestPasswordTokenAsync(request, cancellationToken);
        return EnsureSuccess(response);
    }

    // Istegin erisim token'ini secilen kapsamdaki yeni token'la degistirir; sifre istenmez.
    public virtual async Task<TokenResponse> ExchangeCurrentTokenAsync(
        LoginSelectionModel selection,
        CancellationToken cancellationToken = default)
    {
        var request = new TokenExchangeTokenRequest
        {
            Address = await GetTokenEndpointAsync(cancellationToken),
            ClientId = OpenIddictConsts.AppClientId,
            Scope = OpenIddictConsts.TokenScopes,
            SubjectToken = CurrentAccessToken,
            SubjectTokenType = OpenIddictConstants.TokenTypeIdentifiers.AccessToken
        };
        AddSelection(request.Parameters, selection);
        var response = await httpClientFactory.CreateClient().RequestTokenExchangeTokenAsync(request, cancellationToken);
        return EnsureSuccess(response);
    }

    // Refresh token ile ayni kapsamda yeni token ister.
    public virtual async Task<TokenResponse> RequestRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var request = new RefreshTokenRequest
        {
            Address = await GetTokenEndpointAsync(cancellationToken),
            ClientId = OpenIddictConsts.AppClientId,
            RefreshToken = refreshToken
        };
        var response = await httpClientFactory.CreateClient().RequestRefreshTokenAsync(request, cancellationToken);
        return EnsureSuccess(response);
    }

    // Secilen tenant ve organizasyonu istege ekler; bos alan eklenmez.
    private static void AddSelection(Parameters parameters, LoginSelectionModel selection)
    {
        parameters.AddOptional(OpenIddictConsts.TenantParameter, selection.TenantId?.ToString());
        parameters.AddOptional(OpenIddictConsts.OrganizationParameter, selection.OrganizationId?.ToString());
    }

    // Token ucunun reddini is hatasina cevirir; sunucunun aciklamasi ayrintiya konur.
    private static TokenResponse EnsureSuccess(TokenResponse response)
    {
        if (response.IsError)
            throw new BusinessException(LoginExceptionCodes.TokenRequestFailed, details: response.ErrorDescription);

        return response;
    }
}
