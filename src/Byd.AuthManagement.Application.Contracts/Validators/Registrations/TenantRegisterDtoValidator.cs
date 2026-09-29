using FluentValidation;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Dtos.Registrations;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Registrations;

/// <summary>
/// Tenant acan kayit istegini dogrular.
/// </summary>
// islevi: Tenant adi, kullanici adi, e-posta ve sifre eksik ya da bicimsiz geldiginde istegi reddeder.
// sistemdeki gorevi: Tenant acan kayit endpointinin public girdi validator'idir; tenant adinin tekilligini cerceve,
// sifre kurallarini cercevenin sifre politikasi denetler.
public sealed class TenantRegisterDtoValidator : AbstractValidator<TenantRegisterDto>
{
    public TenantRegisterDtoValidator()
    {
        RuleFor(input => input.TenantName)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.TenantNameRequired)
            .MaximumLength(RegistrationConsts.MaxTenantNameLength)
            .WithErrorCode(RegistrationExceptionCodes.TenantNameTooLong);

        RuleFor(input => input.UserName)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.UserNameRequired);

        RuleFor(input => input.Email)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.EmailRequired)
            .EmailAddress().WithErrorCode(RegistrationExceptionCodes.EmailInvalid);

        RuleFor(input => input.Password)
            .NotEmpty().WithErrorCode(RegistrationExceptionCodes.PasswordRequired);
    }
}
