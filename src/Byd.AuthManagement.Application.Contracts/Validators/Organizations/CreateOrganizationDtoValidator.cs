using Byd.AuthManagement.Dtos.Organizations;

namespace Byd.AuthManagement.Validators.Organizations;

/// <summary>
/// Organizasyon create istegini ortak alan kurallariyla dogrular.
/// </summary>
// islevi: CreateOrganizationDto icin FluentValidation kurallarini etkinlestirir.
// sistemdeki gorevi: Create endpointinin public girdi validator'idir.
public sealed class CreateOrganizationDtoValidator
    : OrganizationInputDtoValidatorBase<CreateOrganizationDto>
{
}
