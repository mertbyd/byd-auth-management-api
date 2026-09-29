using Byd.AuthManagement.Consts;
using Volo.Abp.TextTemplating;

namespace Byd.AuthManagement.Invitations;

/// <summary>
/// Davet e-postasinin sablonunu tanimlar.
/// </summary>
// islevi: Davet sablonunu kultur basina ayri dosyadan (tr.tpl, en.tpl) okunacak sekilde, Turkce varsayilanla tanimlar.
// sistemdeki gorevi: Davet mailcisi sablonu bu adla render eder; email modulu ayni adla veritabanindaki icerigi one koyar.
public class InvitationTemplateDefinitionProvider : TemplateDefinitionProvider
{
    // Davet sablonunu gomulu dosya klasoruyle tanimlar.
    public override void Define(ITemplateDefinitionContext context)
    {
        context.Add(
            new TemplateDefinition(InvitationConsts.EmailTemplateName, defaultCultureName: "tr")
                .WithVirtualFilePath("/Invitations/Templates/UserInvitation", isInlineLocalized: false));
    }
}
