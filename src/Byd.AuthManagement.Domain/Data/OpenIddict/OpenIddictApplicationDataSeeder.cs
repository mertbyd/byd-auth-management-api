using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using Byd.AuthManagement.Consts;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.Uow;

namespace Byd.AuthManagement.Data.OpenIddict;

/// <summary>
/// Token sunucusunun API scope'unu ve uygulama client'ini idempotent olarak olusturur.
/// </summary>
// islevi: Modulun scope'unu ve sifreyle token alan public client'i tabloda yoksa ekler, varsa tanimini gunceller.
// sistemdeki gorevi: Token ucunun kabul edecegi client ile scope'un her veritabaninda bulunmasini saglar.
public class OpenIddictApplicationDataSeeder(
    IConfiguration configuration,
    IOpenIddictApplicationRepository applicationRepository,
    IAbpApplicationManager applicationManager,
    IOpenIddictScopeRepository scopeRepository,
    IOpenIddictScopeManager scopeManager)
    : OpenIddictDataSeedContributorBase(
        configuration,
        applicationRepository,
        applicationManager,
        scopeRepository,
        scopeManager), IDataSeedContributor, ITransientDependency
{
    // Once scope, sonra client olusur; client izinleri scope'u adiyla isaret eder.
    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync(new OpenIddictScopeDescriptor
        {
            Name = OpenIddictConsts.ApiScope,
            DisplayName = OpenIddictConsts.ApiScope,
            Resources = { OpenIddictConsts.ApiScope }
        });

        await CreateOrUpdateApplicationAsync(
            applicationType: OpenIddictConstants.ApplicationTypes.Web,
            name: OpenIddictConsts.AppClientId,
            type: OpenIddictConstants.ClientTypes.Public,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: OpenIddictConsts.AppClientDisplayName,
            secret: null,
            grantTypes:
            [
                OpenIddictConstants.GrantTypes.Password,
                OpenIddictConstants.GrantTypes.RefreshToken,
                OpenIddictConstants.GrantTypes.TokenExchange
            ],
            scopes:
            [
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Phone,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConsts.ApiScope
            ]);
    }
}
