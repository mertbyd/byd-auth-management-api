using System;

namespace Byd.AuthManagement.Models.Invitations;

/// <summary>
/// Davet edilen kullaniciyi tasir.
/// </summary>
// islevi: Davetin sonucundaki kullaniciyi ve yeni acilip acilmadigini tasir.
// sistemdeki gorevi: AccountManager daveti ile davet cevabi arasindaki cikis modelidir; yeni kullaniciya sifre belirleme
// baglantisi gider, mevcut kullaniciya gitmez.
public sealed class InvitedUserModel
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    // Kullanici bu davetle acildi; sifresini davet baglantisiyla belirler.
    public bool IsNewUser { get; set; }
    // Yeni kullanicinin sifre belirleme token'i; mevcut kullanicida bos, cevaba cikmaz.
    public string? PasswordResetToken { get; set; }
}
