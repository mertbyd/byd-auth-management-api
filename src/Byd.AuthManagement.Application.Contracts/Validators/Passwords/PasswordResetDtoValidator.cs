using FluentValidation;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Passwords;

/// <summary>
/// Sifre sifirlama istegini dogrular.
/// </summary>
// islevi: Kullanici, token ve yeni sifre eksik geldiginde istegi reddeder.
// sistemdeki gorevi: Sifre endpointinin public girdi validator'idir; sifre kurallari cercevenin kontrolundedir.
public sealed class PasswordResetDtoValidator : AbstractValidator<PasswordResetDto>
{
    public PasswordResetDtoValidator()
    {
        RuleFor(input => input.UserId)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.UserIdRequired);

        RuleFor(input => input.ResetToken)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.ResetTokenRequired);

        RuleFor(input => input.Password)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.PasswordRequired);
    }
}
