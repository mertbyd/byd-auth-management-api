using System;

namespace Byd.AuthManagement.Dtos.Organizations;

/// <summary>
/// Kalici organizasyon kaydinin public cevap alanlarini tasir.
/// </summary>
// islevi: Organizasyon kimligini, agactaki yerini ve audit zamanlarini HTTP ciktisina acar.
// sistemdeki gorevi: Entity veya EF tiplerini disariya sizdirmayan response sozlesmesidir.
public class OrganizationDto
{
    public Guid Id { get; set; }

    public Guid? TenantId { get; set; }

    public Guid? ParentId { get; set; }

    // Ust organizasyonun adi; kok organizasyonda bos gelir.
    public string? ParentName { get; set; }

    // Agactaki yeri kodlayan alan; sunucu uretir.
    public string Code { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public DateTime CreationTime { get; set; }

    public DateTime? LastModificationTime { get; set; }
}
