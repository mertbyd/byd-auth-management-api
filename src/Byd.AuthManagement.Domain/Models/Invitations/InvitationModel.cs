using System;
using System.Collections.Generic;

namespace Byd.AuthManagement.Models.Invitations;

/// <summary>
/// Kullanici daveti isteginin domain girdisini tasir.
/// </summary>
// islevi: Verilecek rolleri ve davet edilen kullanicinin kimlik bilgilerini tasir.
// sistemdeki gorevi: Mapperly ile AccountManager daveti arasindaki girdi sozlesmesidir.
public sealed class InvitationModel
{
    public List<Guid> RoleIds { get; set; } = [];
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
}
