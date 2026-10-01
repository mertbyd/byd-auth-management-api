using System;

namespace Byd.AuthManagement.Dtos.Logins;

/// <summary>
/// Kullanicinin oturumdaki tenant'ta uyesi oldugu bir organizasyonu tasir.
/// </summary>
// islevi: Organizasyonun kimligini ve adini HTTP ciktisina acar.
// sistemdeki gorevi: Oturum bilgisi cevabinin uyelik satiridir.
public class CurrentLoginOrganizationDto
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; } = default!;
}
