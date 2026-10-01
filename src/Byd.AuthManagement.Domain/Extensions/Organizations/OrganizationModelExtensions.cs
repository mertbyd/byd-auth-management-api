using System.Collections.Generic;
using Byd.AuthManagement.Models.Features;
using Byd.AuthManagement.Models.Organizations;
using Volo.Abp.Identity;

namespace Byd.AuthManagement.Extensions.Organizations;

/// <summary>
/// Organizasyon akışlarının domain modellerine yaptığı saf alan aktarımlarını merkezileştirir.
/// </summary>
// islevi: Olusturulan organizasyon, rol ve kullaniciyi create sonucunda tek yerde birlestirir.
// sistemdeki gorevi: OrganizationManager'in kararlarini degistirmeden tekrar eden sonuc kurulumunu tasir.
public static class OrganizationModelExtensions
{
    // Organizasyon, admin rolu ve yonetici hesabindan create sonucunu kurar.
    public static CreatedOrganizationModel ToCreatedOrganizationModel(
        this OrganizationUnit organization,
        string? parentName,
        IdentityRole adminRole,
        IdentityUser adminUser,
        bool isAdminUserNew,
        List<FeatureSelectionModel> features)
    {
        return new CreatedOrganizationModel
        {
            Id = organization.Id,
            TenantId = organization.TenantId,
            ParentId = organization.ParentId,
            ParentName = parentName,
            Code = organization.Code,
            DisplayName = organization.DisplayName,
            CreationTime = organization.CreationTime,
            LastModificationTime = organization.LastModificationTime,
            AdminRoleId = adminRole.Id,
            AdminRoleName = adminRole.Name,
            AdminUserId = adminUser.Id,
            IsAdminUserNew = isAdminUserNew,
            Features = features
        };
    }
}
