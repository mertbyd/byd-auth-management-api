using FluentValidation;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Logins;

/// <summary>
/// Login istegini dogrular.
/// </summary>
// islevi: Kullanici adi ve sifre bos geldiginde istegi kimlik kontrolune gitmeden reddeder.
// sistemdeki gorevi: Login endpointinin public girdi validator'idir.
public sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(input => input.UserName)
            .NotEmpty().WithErrorCode(LoginExceptionCodes.UserNameRequired);

        RuleFor(input => input.Password)
            .NotEmpty().WithErrorCode(LoginExceptionCodes.PasswordRequired);
    }
}
