using FluentValidation;
using Byd.AuthManagement.Dtos.Invitations;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Invitations;

/// <summary>
/// Kullanici daveti istegini dogrular.
/// </summary>
// islevi: Rol listesi, kullanici adi, e-posta ve uygulama adi eksik ya da bicimsiz geldiginde istegi reddeder.
// sistemdeki gorevi: Davet endpointinin public girdi validator'idir; rollerin varligi ve kapsami Manager'dadir.
public sealed class InviteUserDtoValidator : AbstractValidator<InviteUserDto>
{
    public InviteUserDtoValidator()
    {
        RuleFor(input => input.RoleIds)
            .NotEmpty().WithErrorCode(InvitationExceptionCodes.RoleIdsRequired);

        RuleForEach(input => input.RoleIds)
            .NotEmpty().WithErrorCode(InvitationExceptionCodes.RoleIdsRequired);

        RuleFor(input => input.UserName)
            .NotEmpty().WithErrorCode(InvitationExceptionCodes.UserNameRequired);

        RuleFor(input => input.Email)
            .NotEmpty().WithErrorCode(InvitationExceptionCodes.EmailRequired)
            .EmailAddress().WithErrorCode(InvitationExceptionCodes.EmailInvalid);

        RuleFor(input => input.AppName)
            .NotEmpty().WithErrorCode(InvitationExceptionCodes.AppNameRequired);
    }
}
