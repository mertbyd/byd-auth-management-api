using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.Logins;
using Byd.AuthManagement.Managers.Accounts;
using Byd.AuthManagement.Mappers.Accounts;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Byd.AuthManagement.Services.Accounts;

/// <summary>
/// Oturum akislarini tek bir hesap servisinde yurutur.
/// </summary>
// islevi: Girdileri dogrular; login ve switch kararlarini Manager'a, token isteklerini token istemcisine yonlendirir.
// sistemdeki gorevi: Controller ile hesap dikey kesitleri arasindaki tek public giris noktasidir.
public class AccountAppService : AuthManagementAppService, IAccountAppService
{
    private static readonly AccountMapper Mapper = new();

    private AccountManager Manager =>
        LazyGetRequiredService<AccountManager>();
    private SignInManager<IdentityUser> SignInManager =>
        LazyGetRequiredService<SignInManager<IdentityUser>>();
    private TokenEndpointClient TokenClient =>
        LazyGetRequiredService<TokenEndpointClient>();
    private IOpenIddictTokenManager TokenManager =>
        LazyGetRequiredService<IOpenIddictTokenManager>();
    private IOpenIddictAuthorizationManager AuthorizationManager =>
        LazyGetRequiredService<IOpenIddictAuthorizationManager>();
    private IValidator<LoginDto> LoginValidator =>
        LazyGetRequiredService<IValidator<LoginDto>>();
    private IValidator<SwitchScopeDto> SwitchValidator =>
        LazyGetRequiredService<IValidator<SwitchScopeDto>>();
    private IValidator<RefreshTokenDto> RefreshValidator =>
        LazyGetRequiredService<IValidator<RefreshTokenDto>>();

    // Kimlik bilgilerini dogrular; secenekleri ve token'i dondurur, secim eksikse token kapsamsizdir.
    // Sifre token ucundan once burada da kontrol edilir: kilit sayaci ve hata kodlari burada islenir.
    public virtual async Task<LoginResponseDto> LoginAsync(
        LoginDto input,
        CancellationToken cancellationToken = default)
    {
        await LoginValidator.ValidateAndThrowAsync(input, cancellationToken);
        var user = await Manager.GetUserAsync(input.UserName, cancellationToken);
        var signIn = await SignInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: true);
        Manager.EnsureSignedIn(signIn);
        var scope = await Manager.GetScopeAsync(user.Id, cancellationToken);
        var selection = await Manager.ResolveTokenSelectionAsync(
            user.Id,
            scope,
            input.TenantId,
            input.OrganizationId,
            cancellationToken);
        var token = await TokenClient.RequestPasswordTokenAsync(
            user.UserName,
            input.Password,
            selection,
            cancellationToken);
        return Mapper.Map(scope, token, selection.IsSelectionRequired);
    }

    // TODO: Login ve switch akisi ABP Shared User Accounts dokumani referans alinarak kuruldu:
    // https://abp.io/docs/10.4/modules/account/shared-user-accounts
    // Kullanici host'ta tek hesaptir; login secenekleri ve token'i verir, tenant ve organizasyon switch ile secilir ya da degistirilir.
    // Secimi dogrular, oturumdaki token'i secilen kapsamdaki token'la degistirir ve eski oturumu kapatir.
    public virtual async Task<LoginTokenDto> SwitchAsync(
        SwitchScopeDto input,
        CancellationToken cancellationToken = default)
    {
        await SwitchValidator.ValidateAndThrowAsync(input, cancellationToken);
        var selection = await Manager.ResolveSelectionAsync(
            CurrentUser.GetId(),
            input.TenantId,
            input.OrganizationId,
            cancellationToken);
        var token = await TokenClient.ExchangeCurrentTokenAsync(selection, cancellationToken);
        await LogoutAsync(cancellationToken);
        return Mapper.Map(token);
    }

    // Refresh token ile ayni kapsamda yeni token ister; kapsami token uretilirken handler yeniden dogrular.
    public virtual async Task<LoginTokenDto> RefreshAsync(
        RefreshTokenDto input,
        CancellationToken cancellationToken = default)
    {
        await RefreshValidator.ValidateAndThrowAsync(input, cancellationToken);
        var token = await TokenClient.RequestRefreshTokenAsync(input.RefreshToken, cancellationToken);
        return Mapper.Map(token);
    }

    // Istegi yapan kullanicinin kimligini, kapsamini, rollerini ve uyeliklerini dondurur.
    public virtual async Task<CurrentLoginDto> GetCurrentAsync(CancellationToken cancellationToken = default)
        => Mapper.Map(await Manager.GetCurrentAsync(cancellationToken));

    // Bu login'in yetkilendirme kaydini ve ona bagli token'lari iptal eder; diger oturumlar etkilenmez.
    // Kayit yonetici uzerinden iptal edilir cunku toplu token iptali token onbellegini bosaltmaz.
    public virtual async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        var authorizationId = CurrentUser.FindClaimValue(OpenIddictConstants.Claims.Private.AuthorizationId);
        if (authorizationId is null)
            return;
        await TokenManager.RevokeByAuthorizationIdAsync(authorizationId, cancellationToken);
        var authorization = await AuthorizationManager.FindByIdAsync(authorizationId, cancellationToken);
        if (authorization is not null)
            await AuthorizationManager.TryRevokeAsync(authorization, cancellationToken);
    }
}
