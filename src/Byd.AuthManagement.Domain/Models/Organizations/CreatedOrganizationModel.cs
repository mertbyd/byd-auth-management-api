using System;
using System.Collections.Generic;
using Byd.AuthManagement.Models.Features;

namespace Byd.AuthManagement.Models.Organizations;

/// <summary>
/// Oluşturulan organizasyonu, temel admin rolünü ve yönetici hesabını taşır.
/// </summary>
// islevi: Organizasyon create akisi sonunda olusan tum public sonucu tek domain modelinde toplar.
// sistemdeki gorevi: OrganizationManager ile create cevabi Mapperly eslemesi arasindaki cikis sozlesmesidir.
public sealed class CreatedOrganizationModel : OrganizationModel
{
    public Guid AdminRoleId { get; set; }
    public string AdminRoleName { get; set; } = default!;
    public Guid AdminUserId { get; set; }
    public bool IsAdminUserNew { get; set; }

    public List<FeatureSelectionModel> Features { get; set; } = [];
}
