using System;

namespace Byd.AuthManagement.Dtos.Organizations;

/// <summary>
/// Organizasyon create ve update isteklerinin ortak alanlarini tasir.
/// </summary>
// islevi: Public girdinin degistirilebilir organizasyon alanlarini tanimlar.
// sistemdeki gorevi: FluentValidation ile Mapperly arasindaki ortak request sozlesmesidir.
public abstract class OrganizationInputDto
{
    // Baglanacagi ust organizasyon; kok organizasyon icin bos birakilir.
    public Guid? ParentId { get; set; }

    public string DisplayName { get; set; } = default!;
}
