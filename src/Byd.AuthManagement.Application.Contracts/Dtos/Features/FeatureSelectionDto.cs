namespace Byd.AuthManagement.Dtos.Features;

/// <summary>
/// Bir kapsama atanacak feature adini ve degerini tasir.
/// </summary>
// islevi: Boolean, sayisal ve secimli feature degerlerini ayni public girdi biciminde kabul eder.
// sistemdeki gorevi: Kapsam olusturma istekleri ile ABP feature yonetim zinciri arasindaki deger sozlesmesidir.
public sealed class FeatureSelectionDto
{
    public string Name { get; set; } = default!;

    public string Value { get; set; } = default!;
}
