namespace Byd.AuthManagement.ExceptionCodes;

/// <summary>
/// Feature secimi girdi hatalarinin kararli kodlarini tanimlar.
/// </summary>
// islevi: Feature listesi, adi ve degeri icin public dogrulama kodlarini tutar.
// sistemdeki gorevi: Feature secimi kullanan endpointlerin ayni hata sozlesmesini paylasmasini saglar.
public static class FeatureExceptionCodes
{
    // En az bir feature secilmelidir.
    public const string SelectionRequired = AuthManagementErrorCodes.FeatureGroup + "0001";

    // Feature adi bos gecilemez.
    public const string NameRequired = AuthManagementErrorCodes.FeatureGroup + "0002";

    // Feature degeri bos gecilemez.
    public const string ValueRequired = AuthManagementErrorCodes.FeatureGroup + "0003";

    // Ayni feature bir istekte birden fazla kez bulunamaz.
    public const string NameDuplicated = AuthManagementErrorCodes.FeatureGroup + "0004";

    // Feature secimi ad-deger ciftlerinden olusan bir liste olmalidir.
    public const string SelectionInvalid = AuthManagementErrorCodes.FeatureGroup + "0005";

    // Secilen feature tanimli degildir.
    public const string Undefined = AuthManagementErrorCodes.FeatureGroup + "0006";

    // Feature degeri tanimin deger tipine uymaz.
    public const string ValueInvalid = AuthManagementErrorCodes.FeatureGroup + "0007";

    // Hedef organizasyon istegin tenant'inda ya da yoneticinin alt agacinda degildir.
    public const string OutsideScope = AuthManagementErrorCodes.FeatureGroup + "0008";
}
