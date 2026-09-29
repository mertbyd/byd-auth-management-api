using FluentValidation;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Logins;

/// <summary>
/// Refresh istegini dogrular.
/// </summary>
// islevi: Refresh token bos geldiginde istegi token sunucusuna gitmeden reddeder.
// sistemdeki gorevi: Refresh endpointinin public girdi validator'idir.
public sealed class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(input => input.RefreshToken)
            .NotEmpty().WithErrorCode(LoginExceptionCodes.RefreshTokenRequired);
    }
}
