using FluentValidation;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Logins;

/// <summary>
/// Kapsam degistirme istegini dogrular.
/// </summary>
// islevi: Tenant bos geldiginde istegi token sunucusuna gitmeden reddeder.
// sistemdeki gorevi: Switch endpointinin public girdi validator'idir; secimin kullaniciya uygunlugu Manager'dadir.
public sealed class SwitchScopeDtoValidator : AbstractValidator<SwitchScopeDto>
{
    public SwitchScopeDtoValidator()
    {
        RuleFor(input => input.TenantId)
            .NotEmpty().WithErrorCode(LoginExceptionCodes.TenantIdRequired);
    }
}
