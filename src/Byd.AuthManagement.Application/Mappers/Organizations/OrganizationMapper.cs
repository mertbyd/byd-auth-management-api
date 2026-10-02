using System.Collections.Generic;
using Byd.AuthManagement.Dtos.Features;
using Byd.AuthManagement.Dtos.Organizations;
using Byd.AuthManagement.Models.Features;
using Byd.AuthManagement.Models.Organizations;
using Riok.Mapperly.Abstractions;

namespace Byd.AuthManagement.Mappers.Organizations;

/// <summary>
/// Organizasyon DTO ve domain model eslemelerini tanimlar.
/// </summary>
// islevi: Create ve update girdilerini domain modeline, okuma modelini cevap DTO'suna Mapperly koduyla cevirir.
// sistemdeki gorevi: Organizasyon katmanlari arasindaki alan tasimanin tek sahibidir.
[Mapper]
public partial class OrganizationMapper
{
    public partial OrganizationCreateModel Map(CreateOrganizationDto source);
    public partial OrganizationUpdateModel Map(UpdateOrganizationDto source);
    public partial OrganizationDto Map(OrganizationModel source);
    public partial CreatedOrganizationDto Map(CreatedOrganizationModel source);
    public partial List<OrganizationDto> Map(List<OrganizationModel> source);
    public partial FeatureSelectionModel Map(FeatureSelectionDto source);
    public partial FeatureSelectionDto Map(FeatureSelectionModel source);
}
