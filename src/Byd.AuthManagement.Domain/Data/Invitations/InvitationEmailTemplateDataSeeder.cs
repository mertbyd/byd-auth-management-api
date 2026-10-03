using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Localization;
using Byd.CoreExtensions.Entities.EmailEvents;
using Byd.CoreExtensions.Interfaces;
using Byd.CoreExtensions.Managers;
using Byd.CoreExtensions.Models.EmailEvents;
using Byd.CoreExtensions.Models.EmailTemplates;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.VirtualFileSystem;

namespace Byd.AuthManagement.Data.Invitations;

/// <summary>
/// Davet e-postasinin core olay kaydini ve host kapsamindaki varsayilan sablonlarini idempotent olarak olusturur.
/// </summary>
// islevi: Davet olayi yoksa core'un olay Manager'i ile acar; gomulu her dil icin host kapsaminda davet sablonu yoksa
// core'un sablon Manager'i ile kurar ve kaydeder.
// sistemdeki gorevi: Core sablon seed etmez; olayin varsayilan sablonu olayi kullanan modulundur. Davet gonderimi tenant
// veya organizasyon kendi surumunu acmadiysa bu satira duser. Mevcut satira dokunmaz.
public class InvitationEmailTemplateDataSeeder(
    EmailEventManager emailEventManager,
    IEmailEventRepository emailEventRepository,
    EmailTemplateManager manager,
    IEmailTemplateRepository repository,
    IVirtualFileProvider virtualFileProvider,
    IStringLocalizer<AuthManagementResource> localizer,
    ICurrentTenant currentTenant,
    ICurrentOrganization currentOrganization) : IDataSeedContributor, ITransientDependency
{
    // Tenant olusturulurken calisan seed'i atlar; host kapsaminda her gomulu dil icin eksik sablonu ekler.
    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId is not null)
            return;

        using (currentTenant.Change(null))
        using (currentOrganization.Change(null))
        {
            var emailEvent = await SeedEventAsync(CancellationToken.None);
            foreach (var cultureName in InvitationConsts.EmailTemplateCultureNames)
                await SeedCultureAsync(emailEvent, cultureName, CancellationToken.None);
        }
    }

    // Davet olayini core olay tablosunda arar; yoksa sabitteki adla acar ve kaydeder.
    private async Task<EmailEvent> SeedEventAsync(CancellationToken cancellationToken = default)
    {
        var current = await emailEventManager.FindByNameAsync(InvitationConsts.EmailEventName, cancellationToken);
        if (current is not null)
            return current;

        var model = new EmailEventCreateModel { Name = InvitationConsts.EmailEventName };
        var entity = await emailEventManager.CreateAsync(model, cancellationToken);
        return await emailEventRepository.InsertAsync(entity, autoSave: true, cancellationToken: cancellationToken);
    }

    // Dilin host sablonu yoksa adi ve konuyu o dilde cozer, icerigi gomulu dosyadan okuyup sablonu ekler.
    private async Task SeedCultureAsync(
        EmailEvent emailEvent,
        string cultureName,
        CancellationToken cancellationToken = default)
    {
        var current = await repository.FindAsync(
            template => template.EmailEventId == emailEvent.Id &&
                        template.CultureName == cultureName &&
                        template.TenantId == null &&
                        template.OrganizationId == null,
            cancellationToken: cancellationToken);
        if (current is not null)
            return;

        using (CultureHelper.Use(cultureName))
        {
            var model = new EmailTemplateCreateModel
            {
                EmailEventId = emailEvent.Id,
                CultureName = cultureName,
                Name = localizer[AuthManagementLocalizationKeys.Emails.UserInvitationName],
                Subject = localizer[AuthManagementLocalizationKeys.Emails.UserInvitationSubject],
                Body = await virtualFileProvider
                    .GetFileInfo(string.Format(InvitationConsts.EmailTemplateVirtualPathFormat, cultureName))
                    .ReadAsStringAsync()
            };
            var entity = await manager.CreateAsync(model, cancellationToken);
            await repository.InsertAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        }
    }
}
