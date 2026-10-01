using FluentValidation;
using Byd.AuthManagement.Dtos.Passwords;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Passwords;

/// <summary>
/// Sifirlama baglantisi istegini dogrular.
/// </summary>
// islevi: E-posta ve uygulama adi eksik ya da bicimsiz geldiginde istegi reddeder.
// sistemdeki gorevi: Sifre endpointinin public girdi validator'idir; sifre kurallari cercevenin kontrolundedir.
public sealed class PasswordResetCodeDtoValidator : AbstractValidator<PasswordResetCodeDto>
{
    public PasswordResetCodeDtoValidator()
    {
        RuleFor(input => input.Email)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.EmailRequired)
            .EmailAddress().WithErrorCode(PasswordExceptionCodes.EmailInvalid);

        RuleFor(input => input.AppName)
            .NotEmpty().WithErrorCode(PasswordExceptionCodes.AppNameRequired);
    }
}
