namespace Byd.AuthManagement.Consts;

/// <summary>
/// Token sunucusunun modulce tanimlanan scope, client ve istek parametresi adlarini tutar.
/// </summary>
// islevi: Seed edilen scope ve client adlarini, token isteginin scope ve kapsam parametrelerini tek yerde tutar.
// sistemdeki gorevi: Seed eden, token isteyen ve token'i dogrulayan taraf ayni metni buradan okur.
public static class OpenIddictConsts
{
    // Modulun API scope'u; token'a audience olarak da yazilir.
    public const string ApiScope = "AuthManagement";

    // Refresh token verilmesini isteyen standart scope.
    public const string OfflineAccessScope = "offline_access";

    // Token isteginde istenen scope'lar.
    public const string TokenScopes = ApiScope + " " + OfflineAccessScope;

    // Sifre, refresh token ve token exchange akislariyla token alan public client.
    public const string AppClientId = "AuthManagement_App";

    // Client'in yonetim ekranlarinda gorunen adi.
    public const string AppClientDisplayName = "AuthManagement Application";

    // Token ucunun sunucu adresine gore yolu.
    public const string TokenEndpointPath = "connect/token";

    // Token isteginde secilen tenant'i tasiyan form parametresi.
    public const string TenantParameter = "tenant_id";

    // Token isteginde secilen organizasyonu tasiyan form parametresi.
    public const string OrganizationParameter = "organization_id";
}
