using System;
using System.Collections.Generic;
using Byd.AuthManagement.Dtos.Features;

namespace Byd.AuthManagement.Dtos.Organizations;

/// <summary>
/// Yeni organizasyonu, temel admin rolünü ve yönetici hesabını taşır.
/// </summary>
// islevi: Create organizasyon cevabina bu islemde olusturulan rol ve yonetici sonucunu ekler.
// sistemdeki gorevi: Get ve update cevaplarini degistirmeden create endpointinin genisletilmis cevabidir.
public sealed class CreatedOrganizationDto : OrganizationDto
{
    public Guid AdminRoleId { get; set; }

    public string AdminRoleName { get; set; } = default!;

    public Guid AdminUserId { get; set; }

    public bool IsAdminUserNew { get; set; }

    public List<FeatureSelectionDto> Features { get; set; } = [];
}
