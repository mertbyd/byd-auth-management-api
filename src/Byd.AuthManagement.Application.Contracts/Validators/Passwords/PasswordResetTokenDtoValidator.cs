using FluentValidation;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Passwords;

/// <summary>
/// Sifirlama token'i dogrulama istegini dogrular.
/// </summary>
// islevi: Kullanici ve token eksik geldiginde istegi reddeder.
// sistemdeki gorevi: Sifre endpointinin public girdi validator'idir; sifre kurallari cercevenin kontrolundedir.
public sealed class PasswordResetTokenDtoValidator : AbstractValidator<PasswordResetTokenDto>
{
    public PasswordResetTokenDtoValidator()
    {
        RuleFor(input => input.UserId)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.UserIdRequired);

        RuleFor(input => input.ResetToken)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.ResetTokenRequired);
    }
}
