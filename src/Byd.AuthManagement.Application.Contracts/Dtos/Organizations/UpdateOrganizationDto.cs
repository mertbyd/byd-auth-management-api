using System.Collections.Generic;
using Byd.AuthManagement.Dtos.Features;

namespace Byd.AuthManagement.Dtos.Organizations;

/// <summary>
/// Mevcut organizasyon guncelleme istegini tanimlar.
/// </summary>
// islevi: Organizasyon update endpointinin public girdi tipidir.
// sistemdeki gorevi: Route kimligini payload disinda tutup degistirilebilir alanlari tasir.
public sealed class UpdateOrganizationDto : OrganizationInputDto
{
    public List<FeatureSelectionDto>? Features { get; set; }
}
