using FluentValidation;
using Byd.AuthManagement.Dtos.Features;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Features;

/// <summary>
/// Feature seciminin public alanlarini dogrular.
/// </summary>
// islevi: Feature adi ve degeri bos geldiginde istegi store'a ulasmadan reddeder.
// sistemdeki gorevi: Feature secimi tasiyan ust validator'larin kullandigi tipli alt validatordur.
public sealed class FeatureSelectionDtoValidator : AbstractValidator<FeatureSelectionDto>
{
    public FeatureSelectionDtoValidator()
    {
        RuleFor(input => input.Name)
            .NotEmpty().WithErrorCode(FeatureExceptionCodes.NameRequired);
        RuleFor(input => input.Value)
            .NotEmpty().WithErrorCode(FeatureExceptionCodes.ValueRequired);
    }
}
