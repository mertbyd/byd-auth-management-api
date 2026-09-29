using Duende.IdentityModel.Client;
using Byd.AuthManagement.Dtos.Invitations;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.Dtos.Registrations;
using Byd.AuthManagement.Models.Invitations;
using Byd.AuthManagement.Models.Logins;
using Byd.AuthManagement.Models.Registrations;
using Riok.Mapperly.Abstractions;

namespace Byd.AuthManagement.Mappers.Accounts;

/// <summary>
/// Hesap islemlerinin DTO, domain modeli ve harici servis tipleri arasindaki eslemelerini tanimlar.
/// </summary>
// islevi: Login, kayit ve sifre dikey kesitlerinin alan tasimasini Mapperly koduyla cevirir.
// sistemdeki gorevi: Hesap katmanlari arasindaki alan tasimanin tek sahibidir.
[Mapper]
public partial class AccountMapper
{
    public partial LoginResponseDto Map(LoginScopeModel source, TokenResponse token, bool isSelectionRequired);
    public partial LoginTokenDto Map(TokenResponse source);
    public partial CurrentLoginDto Map(CurrentLoginModel source);
    public partial RegistrationModel Map(RegisterDto source);
    public partial TenantRegistrationModel Map(TenantRegisterDto source);
    public partial RegisteredUserDto Map(RegisteredUserModel source);
    public partial InvitationModel Map(InviteUserDto source);
    public partial InvitedUserDto Map(InvitedUserModel source);
    public partial Volo.Abp.Account.SendPasswordResetCodeDto Map(PasswordResetCodeDto source);
    public partial Volo.Abp.Account.VerifyPasswordResetTokenInput Map(PasswordResetTokenDto source);
    public partial Volo.Abp.Account.ResetPasswordDto Map(PasswordResetDto source);
    public partial Volo.Abp.Account.ChangePasswordInput Map(PasswordChangeDto source);
}
