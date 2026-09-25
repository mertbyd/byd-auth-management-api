namespace Byd.AuthManagement.Consts;

/// <summary>
/// Organizasyon HTTP rotalarini tanimlar.
/// </summary>
// islevi: Organizasyon endpointlerinin kararli rota sozlesmesini tek yerde tutar.
// sistemdeki gorevi: Controller ile istemcilerin ayni derleme guvenli route degerlerini kullanmasini saglar.
public static class OrganizationRoutes
{
    // Organizasyon endpoint ailesinin kok rotasidir.
    public const string Root = "api/auth-management/organizations";

    // Kimlik alan endpointlerin rota kalibidir.
    public const string ById = "{id:guid}";

    // Ust organizasyonu olmayan kayitlari donduren endpointin rotasidir.
    public const string Roots = "roots";

    // Bir organizasyonun dogrudan alt kayitlarini donduren endpointin rota kalibidir.
    public const string Children = "{id:guid}/children";

    // Tenant altindaki tum organizasyonlari donduren endpointin rota kalibidir.
    public const string ByTenant = "tenants/{tenantId:guid}";

    // Tenant altindaki kok organizasyonlari donduren endpointin rota kalibidir.
    public const string TenantRoots = "tenants/{tenantId:guid}/roots";

    // Tenant organizasyonlarini agac kodu sirasinda donduren endpointin rota kalibidir.
    public const string TenantTree = "tenants/{tenantId:guid}/tree";

    // Kullanicinin tenant icindeki organizasyon uyeliklerini donduren endpointin rota kalibidir.
    public const string UserOrganizations = "tenants/{tenantId:guid}/users/{userId:guid}";
}
