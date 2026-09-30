using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Byd.AuthManagement.Dtos.Invitations;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.Dtos.Registrations;
using Byd.AuthManagement.Logins;
using Byd.AuthManagement.Managers.Accounts;
using Byd.AuthManagement.Mappers.Accounts;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Byd.AuthManagement.Services.Accounts;

/// <summary>
/// Oturum, kayit ve sifre akislarini tek bir hesap servisinde yurutur.
/// </summary>
// islevi: Girdileri dogrular; login, switch, kayit ve davet kararlarini Manager'a, sifre islemlerini ve sifre belirleme
// baglantisini cerceve servislerine yonlendirir.
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
    private IValidator<RegisterDto> RegisterValidator =>
        LazyGetRequiredService<IValidator<RegisterDto>>();
    private IValidator<TenantRegisterDto> TenantRegisterValidator =>
        LazyGetRequiredService<IValidator<TenantRegisterDto>>();
    private IValidator<InviteUserDto> InviteUserValidator =>
        LazyGetRequiredService<IValidator<InviteUserDto>>();
    private IAccountEmailer AccountEmailer =>
        LazyGetRequiredService<IAccountEmailer>();
    private Volo.Abp.Account.IAccountAppService AbpAccountAppService =>
        LazyGetRequiredService<Volo.Abp.Account.IAccountAppService>();
    private Volo.Abp.Account.IProfileAppService AbpProfileAppService =>
        LazyGetRequiredService<Volo.Abp.Account.IProfileAppService>();
    private IValidator<PasswordResetCodeDto> ResetCodeValidator =>
        LazyGetRequiredService<IValidator<PasswordResetCodeDto>>();
    private IValidator<PasswordResetTokenDto> ResetTokenValidator =>
        LazyGetRequiredService<IValidator<PasswordResetTokenDto>>();
    private IValidator<PasswordResetDto> ResetValidator =>
        LazyGetRequiredService<IValidator<PasswordResetDto>>();
    private IValidator<PasswordChangeDto> ChangeValidator =>
        LazyGetRequiredService<IValidator<PasswordChangeDto>>();

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

    // Girdiyi dogrular, kullaniciyi host'ta acip secilen tenant'in ya da organizasyonun varsayilan rollerini verir.
    public virtual async Task<RegisteredUserDto> RegisterAsync(
        RegisterDto input,
        CancellationToken cancellationToken = default)
    {
        await RegisterValidator.ValidateAndThrowAsync(input, cancellationToken);
        var user = await Manager.RegisterAsync(Mapper.Map(input), cancellationToken);
        return Mapper.Map(user);
    }

    // Girdiyi dogrular, tenant'i acip kullaniciyi o tenant'in tum izinli sahip rolune baglar.
    public virtual async Task<RegisteredUserDto> RegisterTenantAsync(
        TenantRegisterDto input,
        CancellationToken cancellationToken = default)
    {
        await TenantRegisterValidator.ValidateAndThrowAsync(input, cancellationToken);
        var user = await Manager.RegisterTenantAsync(Mapper.Map(input), cancellationToken);
        return Mapper.Map(user);
    }

    // Girdiyi dogrular, kullaniciyi rolleriyle davet eder; yeni kullaniciya ABP'nin merkezi sifre belirleme
    // e-postasini gonderir.
    public virtual async Task<InvitedUserDto> InviteUserAsync(
        InviteUserDto input,
        CancellationToken cancellationToken = default)
    {
        await InviteUserValidator.ValidateAndThrowAsync(input, cancellationToken);
        var invitation = await Manager.InviteUserAsync(Mapper.Map(input), cancellationToken);
        if (invitation.IsNewUser)
        {
            await AccountEmailer.SendPasswordResetLinkAsync(
                invitation.User,
                invitation.PasswordResetToken!,
                input.AppName,
                input.ReturnUrl,
                input.ReturnUrlHash);
        }

        return Mapper.Map(invitation);
    }

    // Sifirlama baglantisini kullanicinin e-postasina gonderir.
    public virtual async Task SendPasswordResetCodeAsync(
        PasswordResetCodeDto input,
        CancellationToken cancellationToken = default)
    {
        await ResetCodeValidator.ValidateAndThrowAsync(input, cancellationToken);
        using var hostScope = CurrentTenant.Change(null);
        await AbpAccountAppService.SendPasswordResetCodeAsync(Mapper.Map(input));
    }

    // Sifirlama token'inin gecerli olup olmadigini dondurur.
    public virtual async Task<bool> VerifyPasswordResetTokenAsync(
        PasswordResetTokenDto input,
        CancellationToken cancellationToken = default)
    {
        await ResetTokenValidator.ValidateAndThrowAsync(input, cancellationToken);
        using var hostScope = CurrentTenant.Change(null);
        return await AbpAccountAppService.VerifyPasswordResetTokenAsync(Mapper.Map(input));
    }

    // Token'la yeni sifreyi belirler.
    public virtual async Task ResetPasswordAsync(
        PasswordResetDto input,
        CancellationToken cancellationToken = default)
    {
        await ResetValidator.ValidateAndThrowAsync(input, cancellationToken);
        using var hostScope = CurrentTenant.Change(null);
        await AbpAccountAppService.ResetPasswordAsync(Mapper.Map(input));
    }

    // Giris yapmis kullanicinin sifresini degistirir; token hangi tenant'i tasirsa tasisin kullanici host'ta aranir.
    public virtual async Task ChangePasswordAsync(
        PasswordChangeDto input,
        CancellationToken cancellationToken = default)
    {
        await ChangeValidator.ValidateAndThrowAsync(input, cancellationToken);
        using var hostScope = CurrentTenant.Change(null);
        await AbpProfileAppService.ChangePasswordAsync(Mapper.Map(input));
    }
}
