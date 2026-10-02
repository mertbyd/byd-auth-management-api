using System.Collections.Generic;
using Byd.AuthManagement.Models.Features;

namespace Byd.AuthManagement.Models.Organizations;

/// <summary>
/// Mevcut organizasyon kaydinin domain update girdisini tanimlar.
/// </summary>
// islevi: Update DTO eslemesini Manager'in tasima ve ad degistirme akisina tasir.
// sistemdeki gorevi: Organizasyon guncelleme senaryosunun transporttan bagimsiz girdi tipidir.
public sealed class OrganizationUpdateModel : OrganizationInputModel
{
    public List<FeatureSelectionModel>? Features { get; set; }
}
