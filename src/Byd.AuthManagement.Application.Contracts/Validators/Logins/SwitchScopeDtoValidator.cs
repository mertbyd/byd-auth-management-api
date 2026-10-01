using System;
using FluentValidation;
using Byd.AuthManagement.Dtos.Logins;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Logins;

/// <summary>
/// Kapsam degistirme istegini dogrular.
/// </summary>
// islevi: Bos Guid degerlerini reddeder; null tenant host, null organizasyon tenant secimini temsil eder.
// sistemdeki gorevi: Switch endpointinin public girdi validator'idir; secimin kullaniciya uygunlugu Manager'dadir.
public sealed class SwitchScopeDtoValidator : AbstractValidator<SwitchScopeDto>
{
    public SwitchScopeDtoValidator()
    {
        RuleFor(input => input.TenantId)
            .NotEqual(Guid.Empty).WithErrorCode(LoginExceptionCodes.ScopeNotAvailable);
        RuleFor(input => input.OrganizationId)
            .NotEqual(Guid.Empty).WithErrorCode(LoginExceptionCodes.ScopeNotAvailable);
    }
}
