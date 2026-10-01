using System;
using System.Collections.Generic;

namespace Byd.AuthManagement.Dtos.Invitations;

/// <summary>
/// Kullanici daveti istegini tanimlar.
/// </summary>
// islevi: Verilecek rolleri, davet edilen kullanicinin kimlik bilgilerini ve sifre belirleme baglantisinin acilacagi
// istemci uygulamasini tasir.
// sistemdeki gorevi: Davet endpointinin public girdi tipidir; tenant ve organizasyon rollerden cikar, ayrica secilmez.
public sealed class InviteUserDto
{
    // Verilecek roller; tenant rolun kendi tenant'indan, organizasyon rolun bagli oldugu organizasyondan cikar.
    public List<Guid> RoleIds { get; set; } = [];

    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    // Sifre belirleme baglantisinin acilacagi istemci uygulamasinin adi; host bu adla adres tanimlar.
    public string AppName { get; set; } = default!;

    // Sifre belirlendikten sonra istemcinin donecegi adres.
    public string? ReturnUrl { get; set; }

    public string? ReturnUrlHash { get; set; }
}
