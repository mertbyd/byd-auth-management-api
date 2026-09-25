using Byd.AuthManagement.Dtos.Organizations;

namespace Byd.AuthManagement.Validators.Organizations;

/// <summary>
/// Organizasyon update istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: UpdateOrganizationDto icin FluentValidation kurallarini etkinlestirir.
// sistemdeki gorevi: Update endpointinin public girdi validator'idir.
public sealed class UpdateOrganizationDtoValidator
    : OrganizationInputDtoValidatorBase<UpdateOrganizationDto>
{
}
