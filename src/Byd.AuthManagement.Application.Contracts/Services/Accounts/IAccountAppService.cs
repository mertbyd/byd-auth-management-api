using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Dtos.Logins;
using Volo.Abp.Application.Services;

namespace Byd.AuthManagement.Services.Accounts;

/// <summary>
/// Oturum kullanim senaryolarini tek bir hesap servisi altinda tanimlar.
/// </summary>
// islevi: Login, kapsam degistirme, token yenileme, oturum bilgisi ve logout kontratini acar.
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
}
