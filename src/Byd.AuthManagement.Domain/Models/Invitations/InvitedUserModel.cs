using System;
using Volo.Abp.Identity;

namespace Byd.AuthManagement.Models.Invitations;

/// <summary>
/// Davet edilen kullaniciyi tasir.
/// </summary>
// islevi: Davetin sonucundaki kullaniciyi ve yeni acilip acilmadigini tasir.
// sistemdeki gorevi: AccountManager daveti ile davet cevabi arasindaki cikis modelidir; yeni kullaniciya sifre belirleme
// baglantisi gider, mevcut kullaniciya gitmez.
public sealed class InvitedUserModel
{
    public IdentityUser User { get; set; } = default!;
    public Guid UserId => User.Id;
    public string UserName => User.UserName;
    public string Email => User.Email;
    // Kullanici bu davetle acildi; sifresini davet baglantisiyla belirler.
    public bool IsNewUser { get; set; }
    // Yeni kullanicinin sifre belirleme token'i; mevcut kullanicida bos, cevaba cikmaz.
    public string? PasswordResetToken { get; set; }
}
