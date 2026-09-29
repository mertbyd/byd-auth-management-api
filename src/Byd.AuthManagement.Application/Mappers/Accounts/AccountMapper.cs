using Duende.IdentityModel.Client;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.Models.Logins;
using Riok.Mapperly.Abstractions;

namespace Byd.AuthManagement.Mappers.Accounts;

/// <summary>
/// Hesap islemlerinin DTO, domain modeli ve harici servis tipleri arasindaki eslemelerini tanimlar.
/// </summary>
// islevi: Login dikey kesitinin alan tasimasini Mapperly koduyla cevirir.
// sistemdeki gorevi: Hesap katmanlari arasindaki alan tasimanin tek sahibidir.
[Mapper]
public partial class AccountMapper
{
    public partial LoginResponseDto Map(LoginScopeModel source, TokenResponse token, bool isSelectionRequired);
    public partial LoginTokenDto Map(TokenResponse source);
    public partial CurrentLoginDto Map(CurrentLoginModel source);
}
