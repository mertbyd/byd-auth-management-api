using FluentValidation;
using Byd.AuthManagement.Dtos.Registrations;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Registrations;

/// <summary>
/// Kayit istegini dogrular.
/// </summary>
// islevi: Tenant, kullanici adi, e-posta ve sifre eksik ya da bicimsiz geldiginde istegi reddeder.
// sistemdeki gorevi: Kayit endpointinin public girdi validator'idir; sifre kurallari cercevenin kontrolundedir.
public sealed class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(input => input.TenantId)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.TenantIdRequired);

        RuleFor(input => input.UserName)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.UserNameRequired);

        RuleFor(input => input.Email)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.EmailRequired)
            .EmailAddress().WithErrorCode(RegistrationExceptionCodes.EmailInvalid);

        RuleFor(input => input.Password)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.PasswordRequired);
    }
}
