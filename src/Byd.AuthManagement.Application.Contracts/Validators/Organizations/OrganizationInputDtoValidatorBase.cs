using FluentValidation;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Dtos.Organizations;
using Byd.AuthManagement.ExceptionCodes;

namespace Byd.AuthManagement.Validators.Organizations;

/// <summary>
/// Organizasyon create ve update isteklerinin ortak sekil kurallarini tanimlar.
/// </summary>
// islevi: Organizasyon adinin zorunluluk ve uzunluk hatalarini veritabanina gitmeden reddeder.
// sistemdeki gorevi: Create ve update validator'larinin kararli hata kodu kullanan ortak tabanidir.
public abstract class OrganizationInputDtoValidatorBase<TDto> : AbstractValidator<TDto>
    where TDto : OrganizationInputDto
{
    protected OrganizationInputDtoValidatorBase()
    {
        RuleFor(input => input.DisplayName)
            .NotEmpty().WithErrorCode(OrganizationExceptionCodes.DisplayNameRequired)
            .MaximumLength(OrganizationConsts.MaxDisplayNameLength)
            .WithErrorCode(OrganizationExceptionCodes.DisplayNameTooLong);
    }
}
