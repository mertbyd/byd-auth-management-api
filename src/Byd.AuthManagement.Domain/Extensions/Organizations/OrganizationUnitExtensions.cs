using System;
using Volo.Abp.Identity;

namespace Byd.AuthManagement.Extensions.Organizations;

/// <summary>
/// Organizasyon ağacındaki saf kapsam karşılaştırmalarını merkezileştirir.
/// </summary>
// islevi: Bir organizasyonun verilen organizasyonun kendisi veya alt agaci olup olmadigini koduyla belirler.
// sistemdeki gorevi: Hesap ve organizasyon yonetimindeki ayni alt agac kuralinin tek gercek kaynagidir.
public static class OrganizationUnitExtensions
{
    // Hedefin mevcut organizasyonun kendisi veya kod onekli alt kaydi olup olmadigini dondurur.
    public static bool IsWithinScopeOf(
        this OrganizationUnit target,
        OrganizationUnit current,
        bool includeCurrent = true)
    {
        var isCurrent = target.Id == current.Id;
        var isDescendant = target.Code.StartsWith(current.Code + ".", StringComparison.Ordinal);
        return includeCurrent ? isCurrent || isDescendant : isDescendant;
    }
}
