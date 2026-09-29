using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Byd.EmailManagement.Interfaces.Emailing;
using Byd.EmailManagement.Models.Emailing;
using Volo.Abp.DependencyInjection;

namespace Byd.AuthManagement;

/// <summary>
/// E-posta saglayicisinin sirlarini host'un ayarindan saglar.
/// </summary>
// islevi: SMTP kimlik bilgilerini ve Google OAuth sirlarini host config'inden (user-secrets, ortam degiskeni) okur;
// calisirken guncellenen sirri bellekte tutar.
// sistemdeki gorevi: Email modulunun host'tan bekledigi sir deposudur; repoya ve appsettings'e sir yazilmaz.
public class ConfigurationEmailSecretStore(IConfiguration configuration) : IEmailSecretStore, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, EmailSecretModel> _updatedSecrets = new();

    // Calisirken guncellenen sirri, yoksa host config'indeki sirri dondurur.
    public Task<EmailSecretModel?> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        if (_updatedSecrets.TryGetValue(path, out var updated))
            return Task.FromResult<EmailSecretModel?>(updated);

        var section = configuration.GetSection("EmailManagement:Secrets");
        return Task.FromResult<EmailSecretModel?>(new EmailSecretModel
        {
            SmtpUsername = section["SmtpUsername"],
            SmtpPassword = section["SmtpPassword"],
            GoogleClientSecret = section["GoogleClientSecret"],
            GoogleRefreshToken = section["GoogleRefreshToken"]
        });
    }

    // OAuth akisinda guncellenen sirri bellege yazar.
    public Task SetAsync(string path, EmailSecretModel secret, CancellationToken cancellationToken = default)
    {
        _updatedSecrets[path] = secret;
        return Task.CompletedTask;
    }
}
