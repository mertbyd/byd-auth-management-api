using System;

namespace Byd.AuthManagement.Dtos.Invitations;

/// <summary>
/// Davet edilen kullaniciyi tanimlar.
/// </summary>
// islevi: Davetin sonucundaki kullaniciyi ve bu davetle acilip acilmadigini HTTP ciktisina acar.
// sistemdeki gorevi: Davet endpointinin cevabidir.
public class InvitedUserDto
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    // Kullanici bu davetle acildi ve sifre belirleme baglantisi gonderildi.
    public bool IsNewUser { get; set; }
}
