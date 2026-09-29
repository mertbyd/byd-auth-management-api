using System;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Kullanici adi ve sifreyle giris istegini tanimlar.
/// </summary>
// islevi: Kimlik bilgilerini ve istege bagli olarak istemcinin sectigi kapsami tasir.
// sistemdeki gorevi: Login endpointinin public girdi tipidir. Kapsam bos gelirse ve secim gerekiyorsa token kapsamsiz verilir,
// istemci kapsami switch ile secer.
public sealed class LoginDto
{
    // Girilecek tenant; bos birakilabilir.
    public Guid? TenantId { get; set; }

    // Girilecek organizasyon; bos birakilabilir.
    public Guid? OrganizationId { get; set; }

    // Kullanici adi ya da e-posta adresi.
    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;
}
