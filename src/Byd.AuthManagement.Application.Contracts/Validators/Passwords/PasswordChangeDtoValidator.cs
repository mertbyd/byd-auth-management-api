using FluentValidation;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Passwords;

/// <summary>
/// Sifre degistirme istegini dogrular.
/// </summary>
// islevi: Mevcut ve yeni sifre eksik ya da ayni geldiginde istegi reddeder.
// sistemdeki gorevi: Sifre endpointinin public girdi validator'idir; sifre kurallari cercevenin kontrolundedir.
public sealed class PasswordChangeDtoValidator : AbstractValidator<PasswordChangeDto>
{
    public PasswordChangeDtoValidator()
    {
        RuleFor(input => input.CurrentPassword)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.CurrentPasswordRequired);

        RuleFor(input => input.NewPassword)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.PasswordRequired)
            .NotEqual(input => input.CurrentPassword).WithErrorCode(PasswordExceptionCodes.NewPasswordSameAsCurrent);
    }
}
