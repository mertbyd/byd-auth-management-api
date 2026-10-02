using Byd.AuthManagement.Dtos.Organizations;
using FluentValidation;
using Byd.AuthManagement.Validators.Features;

namespace Byd.AuthManagement.Validators.Organizations;

/// <summary>
/// Organizasyon update istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: UpdateOrganizationDto icin FluentValidation kurallarini etkinlestirir.
// sistemdeki gorevi: Update endpointinin public girdi validator'idir.
public sealed class UpdateOrganizationDtoValidator
    : OrganizationInputDtoValidatorBase<UpdateOrganizationDto>
{
    // Secim gonderildiginde liste bicimini ortak feature kurallariyla dogrular.
    public UpdateOrganizationDtoValidator()
    {
        RuleFor(input => input.Features!)
            .SetValidator(new FeatureSelectionListValidator())
            .When(input => input.Features is not null);
    }
}
