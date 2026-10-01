using FluentValidation;
using Byd.AuthManagement.Dtos.Organizations;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Validators.Features;

namespace Byd.AuthManagement.Validators.Organizations;

/// <summary>
/// Organizasyon create istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: CreateOrganizationDto icin FluentValidation kurallarini etkinlestirir.
// sistemdeki gorevi: Create endpointinin public girdi validator'idir.
public sealed class CreateOrganizationDtoValidator
    : OrganizationInputDtoValidatorBase<CreateOrganizationDto>
{
    public CreateOrganizationDtoValidator()
    {
        RuleFor(input => input.AdminUserName)
            .NotEmpty().WithErrorCode(OrganizationExceptionCodes.AdminUserNameRequired);
        RuleFor(input => input.AdminEmail)
            .NotEmpty().WithErrorCode(OrganizationExceptionCodes.AdminEmailRequired)
            .EmailAddress().WithErrorCode(OrganizationExceptionCodes.AdminEmailInvalid);
        RuleFor(input => input.AdminPassword)
            .NotEmpty().WithErrorCode(OrganizationExceptionCodes.AdminPasswordRequired);
        RuleFor(input => input.Features)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(FeatureExceptionCodes.SelectionRequired)
            .SetValidator(new FeatureSelectionListValidator());
    }
}
