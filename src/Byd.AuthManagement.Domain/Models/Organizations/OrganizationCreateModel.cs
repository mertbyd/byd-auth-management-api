using System.Collections.Generic;
using Byd.AuthManagement.Models.Features;

namespace Byd.AuthManagement.Models.Organizations;

/// <summary>
/// Yeni organizasyon kaydinin domain create girdisini tanimlar.
/// </summary>
// islevi: Create DTO eslemesini Manager'in entity kurma akisina tasir.
// sistemdeki gorevi: Organizasyon olusturma senaryosunun transporttan bagimsiz girdi tipidir.
public sealed class OrganizationCreateModel : OrganizationInputModel
{
    public string AdminUserName { get; set; } = default!;
    public string AdminEmail { get; set; } = default!;
    public string AdminPassword { get; set; } = default!;

    public List<FeatureSelectionModel> Features { get; set; } = [];
}
