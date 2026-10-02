using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Dtos.Invitations;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.Dtos.Registrations;
using Byd.AuthManagement.Permissions;
using Byd.AuthManagement.Services.Accounts;
using SystemStandards.Results;

namespace Byd.AuthManagement.Controllers.Accounts;

/// <summary>
/// Oturum, kayit ve sifre yonetimi islemlerini HTTP uzerinden sunar.
/// </summary>
// islevi: Route ve request binding metadatasini tasiyip her istegi tek hesap AppService cagrisina yonlendirir.
// sistemdeki gorevi: ABP Account yapisina uygun, hesap dikey kesitinin ince HTTP giris noktasidir.
[ApiController]
[Route(AccountRoutes.Root)]
public class AccountController : AuthManagementController
{
    /// <summary>Hesap AppService'ini lazy cozer.</summary>
    private IAccountAppService AppService =>
        LazyGetRequiredService<IAccountAppService>();

    /// <summary>
    /// Kullanici adi ve sifreyle giris yapar.
    /// </summary>
    /// <param name="input">Kimlik bilgileri ve istemcinin sectigi tenant ile organizasyon.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { TenantId, OrganizationId, UserName, Password }. UserName yerine e-posta da verilebilir. TenantId ve
    /// OrganizationId bos gonderilebilir; kapsam sonradan switch ile secilir.
    /// Response value { IsSelectionRequired, Tenants, Organizations, Token } tasir. Tenants kullanicinin girebilecegi
    /// tenant'lardir { Id, Name, HasRole }; Id ve Name bos satir host'tur. Organizations uyeliklerdir
    /// { Id, TenantId, ParentId, ParentName, Code, DisplayName, HasRole }. Token { AccessToken, TokenType, RefreshToken,
    /// ExpiresIn } her zaman doner.
    /// Birden fazla tenant'ta ya da secilen tenant'ta birden fazla organizasyonda secim yapilmadiysa IsSelectionRequired
    /// true olur ve token kapsamsizdir: tenant, organizasyon ve rol tasimaz; yalniz me, switch, logout ve sifre
    /// degistirme uclarinda gecer. Istemci bu token'la switch ucuna secimini gonderip kapsamli token alir. Secim tamamsa
    /// ya da tek secenek varsa token dogrudan kapsamlidir; secilen kapsamin rollerini ve tenant_id, tenant_name,
    /// organization_id, organization_name ve scope_level claim'lerini icerir.
    /// Hatali denemeler hesabi gecici olarak kilitler. Secilen kapsamda rolu olmayan kullaniciya token verilmez.
    /// </remarks>
    [HttpPost(AccountRoutes.Login)]
    [AllowAnonymous]
    public virtual async Task<Result<LoginResponseDto>> LoginAsync(
        [FromBody] LoginDto input,
        CancellationToken cancellationToken = default)
        => await AppService.LoginAsync(input, cancellationToken);

    /// <summary>
    /// Oturumdaki token'la tenant ve organizasyon secer ya da degistirir.
    /// </summary>
    /// <param name="input">Gecilecek tenant ve organizasyon.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { TenantId, OrganizationId }; ikisi de null ise host, yalniz OrganizationId null ise tenant secilir. Sifre istenmez,
    /// kullanici istegin erisim token'indan okunur; login'in kapsamsiz token'i da kapsamli token da kullanilabilir.
    /// Response value { AccessToken, TokenType, RefreshToken, ExpiresIn } tasir; erisim token'i secilen kapsamin
    /// rollerini ve tenant_id, tenant_name, organization_id, organization_name claim'lerini icerir. Istekteki token ve
    /// ona bagli refresh token iptal edilir; istemci yeni token'a gecer. Secim kullanicinin kapsamlari arasinda degilse
    /// ya da kapsamda rolu yoksa token verilmez ve eski token gecerli kalir.
    /// </remarks>
    [HttpPost(AccountRoutes.Switch)]
    [Authorize]
    public virtual async Task<Result<LoginTokenDto>> SwitchAsync(
        [FromBody] SwitchScopeDto input,
        CancellationToken cancellationToken = default)
        => await AppService.SwitchAsync(input, cancellationToken);

    /// <summary>
    /// Refresh token ile yeni token alir.
    /// </summary>
    /// <param name="input">Onceki login'de verilen refresh token.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { RefreshToken }. Response value { AccessToken, TokenType, RefreshToken, ExpiresIn } tasir.
    /// Kapsam refresh token'in icinden okunur ve yeniden dogrulanir; kullanici organizasyondan cikarildiysa,
    /// kapsamdaki rolleri alindiysa ya da logout olduysa yeni token verilmez. Kapsam degistirmek icin switch ucu
    /// kullanilir.
    /// </remarks>
    [HttpPost(AccountRoutes.Refresh)]
    [AllowAnonymous]
    public virtual async Task<Result<LoginTokenDto>> RefreshAsync(
        [FromBody] RefreshTokenDto input,
        CancellationToken cancellationToken = default)
        => await AppService.RefreshAsync(input, cancellationToken);

    /// <summary>
    /// Istegi yapan kullanicinin kimligini, kapsamini, rollerini ve uyeliklerini getirir.
    /// </summary>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value { TenantId, TenantName, OrganizationId, OrganizationName, ScopeLevel, UserId, UserName, Name,
    /// Surname, Email, Roles, Organizations } tasir. ScopeLevel JSON'da SuperAdmin, Tenant, Organization veya
    /// SubOrganization sayisal enum degeri olarak gorunur; secilmemis oturum Unselected (0) doner.
    /// Roles elemanlari { Id, Name, ScopeLevel } tasir. Kapsam seviyesi scope_level claim'inden okunur. Organizations
    /// kullanicinin oturumdaki tenant'ta uyesi oldugu organizasyonlardir, elemanlari { Id, DisplayName } tasir;
    /// uyelik yoksa bos liste doner. Host kapsaminda TenantId ve TenantName bos gelir. Rolleri token alindiktan
    /// sonra degisen kullanicinin yeni rolleri refresh ya da yeniden login ile gelir.
    /// </remarks>
    [HttpGet(AccountRoutes.Me)]
    [Authorize]
    public virtual async Task<Result<CurrentLoginDto>> GetCurrentAsync(CancellationToken cancellationToken = default)
        => await AppService.GetCurrentAsync(cancellationToken);

    /// <summary>
    /// Istegin ait oldugu oturumu kapatir.
    /// </summary>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Istekteki erisim token'i ile ayni login'den gelen butun token'lar (erisim ve refresh) iptal edilir; iptal edilen
    /// erisim token'i bu host'ta bir sonraki istekte reddedilir, refresh token ile yeni token alinamaz. Kullanicinin
    /// baska cihazdaki oturumlari etkilenmez. Token'i yalniz imzasiyla dogrulayan diger modul host'lari iptali goremez;
    /// orada erisim token'i suresi dolana kadar gecerli kalir. Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpPost(AccountRoutes.Logout)]
    [Authorize]
    public virtual async Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
    {
        await AppService.LogoutAsync(cancellationToken);
        return Result.NoContent();
    }

    /// <summary>
    /// Yeni kullaniciyi sistemdeki aktif tenant'a ve tenant rollerine kaydeder.
    /// </summary>
    /// <param name="input">Kullanici adi, e-posta ve sifre.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { UserName, Email, Password }. Kullanici host'ta acilir, sistemdeki aktif tenant belirlenir ve tenant rolleri UserRoles ile eslenir.
    /// Response value { TenantId, TenantName, UserId, UserName, Email } tasir.
    /// Kullanici adi ve e-posta sistem genelinde tekildir; sifre cercevenin sifre kurallarina uymalidir.
    /// </remarks>
    [HttpPost(AccountRoutes.Register)]
    [AllowAnonymous]
    public virtual async Task<Result<RegisteredUserDto>> RegisterAsync(
        [FromBody] RegisterDto input,
        CancellationToken cancellationToken = default)
        => await AppService.RegisterAsync(input, cancellationToken);

    /// <summary>
    /// Tenant acarak yeni kullaniciyi kaydeder.
    /// </summary>
    /// <param name="input">Acilacak tenant'in adi, kullanici adi, e-posta ve sifre.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { TenantName, UserName, Email, Password }. Tenant acilir, tenant'ta uygulamanin tenant tarafindaki tum
    /// izinlerini tasiyan admin rolu kurulur; kullanici host'ta acilir, bu role girer ve hemen login olabilir.
    /// Response value { TenantId, TenantName, UserId, UserName, Email } tasir. Admin rolu varsayilan rol degildir;
    /// tenant'a sonradan katilanlara verilmez. Feature secimi kabul etmez ve override yazmaz; ABP feature
    /// varsayilanlari kullanilir. Organizasyon acmaz. Tenant adi, kullanici adi
    /// ve e-posta sistem genelinde tekildir; sifre cercevenin sifre kurallarina uymalidir.
    /// </remarks>
    [HttpPost(AccountRoutes.RegisterTenant)]
    [AllowAnonymous]
    public virtual async Task<Result<RegisteredUserDto>> RegisterTenantAsync(
        [FromBody] TenantRegisterDto input,
        CancellationToken cancellationToken = default)
        => await AppService.RegisterTenantAsync(input, cancellationToken);

    /// <summary>
    /// Kullaniciyi rolleriyle davet eder.
    /// </summary>
    /// <param name="input">Verilecek roller, kullanici adi, e-posta ve sifre belirleme baglantisinin acilacagi uygulama.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { RoleIds, UserName, Email, AppName, ReturnUrl, ReturnUrlHash }. Tenant her rolun kendi tenant'idir;
    /// butun roller kendi tenant'inda dogrudan verilir; organizasyona bagli rolde ayrica secilebilirlik uyeligi acilir.
    /// Tenant'taki davet eden yalniz kendi tenant'inin rollerini, super admin her tenant'in rollerini verebilir.
    /// Organizasyon yoneticisi yalniz kendi organizasyonu veya alt agacina bagli rolleri verebilir; tenant ya da baska
    /// organizasyon rolleri 050001 ile reddedilir. Bir rol birden fazla organizasyona bagliysa davet yapilmaz.
    /// E-posta kayitliysa mevcut kullanici kullanilir, rolleri eklenir ve mail gitmez. Degilse kullanici host'ta
    /// sifresiz acilir ve AppName'in sifre sifirlama sayfasina giden bir sifre belirleme baglantisi e-postasina
    /// gonderilir; kullanici sifresini reset-password ucuyla belirleyip login olur.
    /// Response value { UserId, UserName, Email, IsNewUser } tasir. Kullaniciyi roller disinda degistirmez, rol
    /// kaldirmaz.
    /// </remarks>
    [HttpPost(AccountRoutes.InviteUser)]
    [Authorize(AuthManagementPermissions.Users.Manage)]
    public virtual async Task<Result<InvitedUserDto>> InviteUserAsync(
        [FromBody] InviteUserDto input,
        CancellationToken cancellationToken = default)
        => await AppService.InviteUserAsync(input, cancellationToken);

    /// <summary>
    /// Sifre sifirlama baglantisini kullanicinin e-postasina gonderir.
    /// </summary>
    /// <param name="input">Kullanicinin e-postasi ve baglantinin acilacagi istemci uygulamasi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { Email, AppName, ReturnUrl, ReturnUrlHash }. Baglanti istemcinin sifre sifirlama sayfasina gider ve
    /// userId ile resetToken tasir; adresi host AppName icin tanimlar. E-postaya kayitli kullanici yoksa hata doner.
    /// Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpPost(AccountRoutes.SendPasswordResetCode)]
    [AllowAnonymous]
    public virtual async Task<Result> SendPasswordResetCodeAsync(
        [FromBody] PasswordResetCodeDto input,
        CancellationToken cancellationToken = default)
    {
        await AppService.SendPasswordResetCodeAsync(input, cancellationToken);
        return Result.NoContent();
    }

    /// <summary>
    /// Sifre sifirlama token'inin gecerli olup olmadigini dondurur.
    /// </summary>
    /// <param name="input">Sifirlama baglantisindaki kullanici ve token.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { UserId, ResetToken }. Response value token gecerliyse true, degilse false tasir. Sifre degistirilmez;
    /// istemci sifre sayfasini acmadan once baglantiyi kontrol etmek icin kullanir.
    /// </remarks>
    [HttpPost(AccountRoutes.VerifyPasswordResetToken)]
    [AllowAnonymous]
    public virtual async Task<Result<bool>> VerifyPasswordResetTokenAsync(
        [FromBody] PasswordResetTokenDto input,
        CancellationToken cancellationToken = default)
        => await AppService.VerifyPasswordResetTokenAsync(input, cancellationToken);

    /// <summary>
    /// Sifre sifirlama token'iyla yeni sifreyi belirler.
    /// </summary>
    /// <param name="input">Sifirlama baglantisindaki kullanici, token ve yeni sifre.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { UserId, ResetToken, Password }. Yeni sifre cercevenin sifre kurallarina uymalidir; token bir kez
    /// kullanilir. Kullanicinin acik oturumlari kapatilmaz. Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpPost(AccountRoutes.ResetPassword)]
    [AllowAnonymous]
    public virtual async Task<Result> ResetPasswordAsync(
        [FromBody] PasswordResetDto input,
        CancellationToken cancellationToken = default)
    {
        await AppService.ResetPasswordAsync(input, cancellationToken);
        return Result.NoContent();
    }

    /// <summary>
    /// Giris yapmis kullanicinin sifresini degistirir.
    /// </summary>
    /// <param name="input">Mevcut ve yeni sifre.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { CurrentPassword, NewPassword }. Mevcut sifre dogru olmali, yeni sifre ondan farkli olmali ve
    /// cercevenin sifre kurallarina uymalidir. Acik oturumlar kapatilmaz. Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpPost(AccountRoutes.ChangePassword)]
    [Authorize]
    public virtual async Task<Result> ChangePasswordAsync(
        [FromBody] PasswordChangeDto input,
        CancellationToken cancellationToken = default)
    {
        await AppService.ChangePasswordAsync(input, cancellationToken);
        return Result.NoContent();
    }
}
