using System;

namespace Byd.AuthManagement.Models.Organizations;

/// <summary>
/// Organizasyon create ve update islemlerinin ortak domain girdisini tasir.
/// </summary>
// islevi: Public DTO alanlarini entity mutasyonundan ayiran tipli domain modelini tanimlar.
// sistemdeki gorevi: Mapperly ile OrganizationManager arasindaki ortak girdi sozlesmesidir.
public abstract class OrganizationInputModel
{
    public Guid? ParentId { get; set; }
    public string DisplayName { get; set; } = default!;
}
