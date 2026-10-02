namespace Byd.AuthManagement.Dtos.Registrations;

/// <summary>
/// Kendi tenant'ini acarak kayit istegini tanimlar.
/// </summary>
// islevi: Acilacak tenant'in adini ve yeni kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Anonim kayitta organizasyon ve feature secimi kabul etmez; feature'lar ABP varsayilanlarindan gelir.
public sealed class TenantRegisterDto
{
    // Acilacak tenant'in adi.
    public string TenantName { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}
