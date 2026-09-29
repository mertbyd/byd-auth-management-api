using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Dtos.Invitations;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.Dtos.Registrations;
using Volo.Abp.Application.Services;

namespace Byd.AuthManagement.Services.Accounts;

/// <summary>
/// Oturum, kayit ve sifre kullanim senaryolarini tek bir hesap servisi altinda tanimlar.
/// </summary>
// islevi: Login, kapsam degistirme, token yenileme, oturum bilgisi, logout, kayit ve sifre yonetimi kontratini acar.
// sistemdeki gorevi: Controller ile Application implementasyonu arasindaki public hesap servisi siniridir.
public interface IAccountAppService : IApplicationService
{
    // Kullanici adi ve sifreyle giris yapar; secenekleri ve token'i dondurur, secim eksikse token kapsamsizdir.
    Task<LoginResponseDto> LoginAsync(LoginDto input, CancellationToken cancellationToken = default);

    // Oturumdaki token'la tenant ve organizasyon secer ya da degistirir; yeni kapsamdaki token'i dondurur.
    Task<LoginTokenDto> SwitchAsync(SwitchScopeDto input, CancellationToken cancellationToken = default);

    // Refresh token ile yeni token alir.
    Task<LoginTokenDto> RefreshAsync(RefreshTokenDto input, CancellationToken cancellationToken = default);

    // Istegi yapan kullanicinin kimligini, kapsamini, rollerini ve uyeliklerini dondurur.
    Task<CurrentLoginDto> GetCurrentAsync(CancellationToken cancellationToken = default);

    // Istegin ait oldugu oturumu ve yetkilendirmeyi kapatir.
    Task LogoutAsync(CancellationToken cancellationToken = default);

    // Yeni kullaniciyi host'ta acar ve secilen tenant'in varsayilan rollerini verir.
    Task<RegisteredUserDto> RegisterAsync(RegisterDto input, CancellationToken cancellationToken = default);

    // Tenant acar ve yeni kullaniciyi o tenant'in tum izinli sahip rolune baglar.
    Task<RegisteredUserDto> RegisterTenantAsync(TenantRegisterDto input, CancellationToken cancellationToken = default);

    // Kullaniciyi rolleriyle davet eder; yeni kullaniciya sifre belirleme baglantisi gonderir.
    Task<InvitedUserDto> InviteUserAsync(InviteUserDto input, CancellationToken cancellationToken = default);

    // Sifre sifirlama baglantisini kullanicinin e-postasina gonderir.
    Task SendPasswordResetCodeAsync(PasswordResetCodeDto input, CancellationToken cancellationToken = default);

    // Sifre sifirlama token'inin gecerli olup olmadigini dondurur.
    Task<bool> VerifyPasswordResetTokenAsync(PasswordResetTokenDto input, CancellationToken cancellationToken = default);

    // Token'la yeni sifreyi belirler.
    Task ResetPasswordAsync(PasswordResetDto input, CancellationToken cancellationToken = default);

    // Giris yapmis kullanicinin sifresini degistirir.
    Task ChangePasswordAsync(PasswordChangeDto input, CancellationToken cancellationToken = default);
}
