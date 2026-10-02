using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Byd.AuthManagement.Dtos.Features;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Features;

// islevi: Feature secim listesinin bicimini ve isim tekilligini dogrular.
// sistemdeki gorevi: Tenant ve organizasyon girdilerinin ortak liste kurallarini uygular.
public sealed class FeatureSelectionListValidator : AbstractValidator<List<FeatureSelectionDto>>
{
    // Bos listeyi kabul eder; null elemanlari ve tekrar eden adlari reddeder.
    public FeatureSelectionListValidator()
    {
        RuleFor(features => features)
            .Must(features => features.Select(feature => feature?.Name)
                .Distinct(StringComparer.Ordinal).Count() == features.Count)
            .WithErrorCode(FeatureExceptionCodes.NameDuplicated);
        RuleForEach(features => features)
            .NotNull().WithErrorCode(FeatureExceptionCodes.SelectionInvalid)
            .SetValidator(new FeatureSelectionDtoValidator());
    }
}
