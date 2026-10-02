using System;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Oturumdaki token'la kapsam secme ya da degistirme istegini tanimlar.
/// </summary>
// islevi: Gecilecek tenant'i ve organizasyonu tasir.
// sistemdeki gorevi: Switch endpointinin public girdi tipidir; kullanici istegin token'indan okunur, sifre istenmez.
public sealed class SwitchScopeDto
{
    // Null host secimidir; secimin kullaniciya acik oldugunu Manager dogrular.
    public Guid? TenantId { get; set; }

    // Null organizasyon, secilen tenant ya da host kapsaminda kalir.
    public Guid? OrganizationId { get; set; }
}
