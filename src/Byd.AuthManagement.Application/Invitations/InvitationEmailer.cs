using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Localization;
using Byd.AuthManagement.Models.Invitations;
using Volo.Abp.Account.Emailing;
using Volo.Abp.Account.Localization;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;

namespace Byd.AuthManagement.Invitations;

/// <summary>
/// Davet edilen yeni kullaniciya sifre belirleme baglantisini gonderir.
/// </summary>
// islevi: Baglantiyi cercevenin sifre sifirlama baglantisiyla ayni bicimde kurar; davet sablonunu istegin kulturunde
// render edip gonderir.
// sistemdeki gorevi: Davet akisinin e-posta adimidir; baglanti istemcinin sifre sayfasina, oradan reset-password ucuna
// gider. Kullanici host'ta durdugu icin baglantidaki tenant bostur.
public class InvitationEmailer(
    IEmailSender emailSender,
    ITemplateRenderer templateRenderer,
    IStringLocalizer<AccountResource> accountLocalizer,
    IStringLocalizer<AuthManagementResource> localizer,
    IAppUrlProvider appUrlProvider,
    ICurrentTenant currentTenant)
    : AccountEmailer(emailSender, templateRenderer, accountLocalizer, appUrlProvider, currentTenant)
{
    // Yeni kullaniciya davet sablonuyla sifre belirleme baglantisini gonderir.
    public virtual async Task SendInvitationLinkAsync(
        InvitedUserModel invitation,
        string appName,
        string? returnUrl,
        string? returnUrlHash,
        CancellationToken cancellationToken = default)
    {
        var link = await BuildPasswordLinkAsync(invitation, appName, returnUrl, returnUrlHash, cancellationToken);
        var body = await TemplateRenderer.RenderAsync(
            InvitationConsts.EmailTemplateName,
            new { name = invitation.UserName, link });
        await EmailSender.SendAsync(
            invitation.Email,
            localizer[AuthManagementLocalizationKeys.Emails.UserInvitationSubject],
            body);
    }

    // Baglantiyi istemcinin sifre sayfasi adresine kullanici, bos tenant ve token ile kurar; donus adresi varsa ekler.
    private async Task<string> BuildPasswordLinkAsync(
        InvitedUserModel invitation,
        string appName,
        string? returnUrl,
        string? returnUrlHash,
        CancellationToken cancellationToken = default)
    {
        var url = await AppUrlProvider.GetResetPasswordUrlAsync(appName);
        var link = $"{url}?userId={invitation.UserId}&{TenantResolverConsts.DefaultTenantKey}=" +
                   $"&resetToken={UrlEncoder.Default.Encode(invitation.PasswordResetToken!)}";
        if (!string.IsNullOrEmpty(returnUrl))
            link += "&returnUrl=" + NormalizeReturnUrl(returnUrl);

        if (!string.IsNullOrEmpty(returnUrlHash))
            link += "&returnUrlHash=" + returnUrlHash;

        return link;
    }
}
