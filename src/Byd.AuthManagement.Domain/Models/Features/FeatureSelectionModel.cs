namespace Byd.AuthManagement.Models.Features;

/// <summary>
/// Bir kapsama yazilacak feature adini ve degerini tasir.
/// </summary>
// islevi: Public feature secimini transport tiplerinden bagimsiz olarak domain akisina aktarir.
// sistemdeki gorevi: Mapperly ile Manager'in ABP feature yonetim cagirilari arasindaki girdi modelidir.
public sealed class FeatureSelectionModel
{
    public string Name { get; set; } = default!;

    public string Value { get; set; } = default!;
}
