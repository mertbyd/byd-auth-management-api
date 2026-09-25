namespace Byd.AuthManagement.Consts;

/// <summary>
/// Modulun veritabani sema adlarini tanimlar.
/// </summary>
// islevi: Auth modulunun kendi tablolarinin yazildigi sema adini derleme guvenli tek sabitte tutar.
// sistemdeki gorevi: Sema adinin tek kaynagidir; DbProperties ve EF konfigurasyonlari ayni sabiti okur,
// tuketen host'un ve diger modullerin semalarina dokunmaz.
public static class DatabaseConsts
{
    // Auth modulunun kendi tablolarinin yazildigi sema.
    public const string ModuleSchema = "auth_management";
}
