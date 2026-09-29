using System;

namespace Byd.AuthManagement.Models.Registrations;

/// <summary>
/// Kaydedilen kullaniciyi ve kaydoldugu tenant'i tasir.
/// </summary>
// islevi: Kayit sonucunun kullanici ve tenant bilgisini tasir.
// sistemdeki gorevi: AccountManager kaydi ile kayit cevabi arasindaki cikis modelidir.
public sealed class RegisteredUserModel
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = default!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
}
