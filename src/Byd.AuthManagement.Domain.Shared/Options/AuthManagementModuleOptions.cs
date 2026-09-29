namespace Byd.AuthManagement.Options;

/// <summary>
/// Modulun host tarafindan beyan edilen calisma ayarlarini tasir.
/// </summary>
// islevi: Token sunucusunun temel adresini tasir.
// sistemdeki gorevi: Modul config okumaz; host adresi kendi ayarindan okuyup buraya yazar.
public class AuthManagementModuleOptions
{
    // Token sunucusunun temel adresi; ornegin https://localhost:44323/
    public string Authority { get; set; } = string.Empty;
}
