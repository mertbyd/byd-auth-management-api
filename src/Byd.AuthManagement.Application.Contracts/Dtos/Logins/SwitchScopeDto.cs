using System;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Oturumdaki token'la kapsam secme ya da degistirme istegini tanimlar.
/// </summary>
// islevi: Gecilecek tenant'i ve organizasyonu tasir.
// sistemdeki gorevi: Switch endpointinin public girdi tipidir; kullanici istegin token'indan okunur, sifre istenmez.
public sealed class SwitchScopeDto
{
    public Guid TenantId { get; set; }

    // Gecilecek organizasyon; tenant'ta tek uyelik varsa bos birakilabilir.
    public Guid? OrganizationId { get; set; }
}
