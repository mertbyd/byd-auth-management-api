using System;

namespace Byd.AuthManagement.Models.Organizations;

/// <summary>
/// Organizasyon kaydini ust kaydinin adiyla birlikte tasir.
/// </summary>
// islevi: Organizasyonun kimligini, agactaki yerini, ust kaydinin adini ve audit zamanlarini tek satirda tasir.
// sistemdeki gorevi: Repository projeksiyonunun cikisidir; ust ad ayri sorguyla cozulmez.
public class OrganizationModel
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ParentId { get; set; }
    public string? ParentName { get; set; }
    public string Code { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool HasRole { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
