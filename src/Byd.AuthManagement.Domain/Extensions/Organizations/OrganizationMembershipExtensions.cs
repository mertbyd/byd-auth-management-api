using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Byd.AuthManagement.Extensions.Organizations;

/// <summary>
/// Organizasyon üyeliği işlemlerinin tenant bağlamını merkezileştirir.
/// </summary>
// islevi: Kullanici-organizasyon uyeligini organizasyonun tenant baglaminda ekler veya kaldirir.
// sistemdeki gorevi: Farkli manager akislarinin ayni ABP tenant degistirme mekanigini tekrar etmesini onler.
public static class OrganizationMembershipExtensions
{
    // Kullaniciyi organizasyonun tenant baglaminda uye yapar.
    public static async Task AddToOrganizationAsync(
        this IdentityUserManager userManager,
        ICurrentTenant currentTenant,
        IdentityUser user,
        OrganizationUnit organization,
        CancellationToken cancellationToken = default)
    {
        using (currentTenant.Change(organization.TenantId))
            await userManager.AddToOrganizationUnitAsync(user, organization);
    }

    // Kullaniciyi organizasyonun tenant baglaminda uyelikten cikarir.
    public static async Task RemoveFromOrganizationAsync(
        this IdentityUserManager userManager,
        ICurrentTenant currentTenant,
        IdentityUser user,
        OrganizationUnit organization,
        CancellationToken cancellationToken = default)
    {
        using (currentTenant.Change(organization.TenantId))
            await userManager.RemoveFromOrganizationUnitAsync(user, organization);
    }
}
