using Microsoft.EntityFrameworkCore;
using Byd.CoreExtensions.EntityFrameworkCore;
using Byd.CoreExtensions.Extensions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Authorizations;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.OpenIddict.Tokens;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Byd.AuthManagement.EntityFrameworkCore;

// Modul kimlik, token, izin ve kiraci tablolarini devralir; ReplaceDbContext beyanlari cercevenin kendi
// context'lerini bu tipe yonlendirir. Dort konu ayri context'lere bolunmez cunku modulun kendi tablolari
// kullaniciya, role ve organizasyona gercek yabanci anahtarla baglanacak; EF Core yabanci anahtari yalnizca
// ayni model icinde kurabilir, ayri context'lerde kalan iki tablo arasinda anahtar uretemez. Ayni model
// ayrica tek migration seti ve tek snapshot demektir, dolayisiyla tablolar birlikte yasar ve birlikte evrilir.
[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(IOpenIddictDbContext))]
[ReplaceDbContext(typeof(IPermissionManagementDbContext))]
[ReplaceDbContext(typeof(IFeatureManagementDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName(AuthManagementDbProperties.ConnectionStringName)]
public class AuthManagementDbContext : BydDbContext<AuthManagementDbContext>,
    IAuthManagementDbContext,
    IIdentityDbContext,
    IOpenIddictDbContext,
    IPermissionManagementDbContext,
    IFeatureManagementDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    // Kimlik entity'leri cerceveden gelir; IIdentityDbContext bu sekiz seti talep eder.
    public DbSet<IdentityUser> Users { get; set; } = default!;

    public DbSet<IdentityRole> Roles { get; set; } = default!;

    public DbSet<IdentityClaimType> ClaimTypes { get; set; } = default!;

    public DbSet<OrganizationUnit> OrganizationUnits { get; set; } = default!;

    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; } = default!;

    public DbSet<IdentityLinkUser> LinkUsers { get; set; } = default!;

    public DbSet<IdentityUserDelegation> UserDelegations { get; set; } = default!;

    public DbSet<IdentitySession> Sessions { get; set; } = default!;

    // Token entity'leri cerceveden gelir; IOpenIddictDbContext bu dort seti talep eder.
    public DbSet<OpenIddictApplication> Applications { get; set; } = default!;

    public DbSet<OpenIddictAuthorization> Authorizations { get; set; } = default!;

    public DbSet<OpenIddictScope> Scopes { get; set; } = default!;

    public DbSet<OpenIddictToken> Tokens { get; set; } = default!;

    // Izin entity'leri cerceveden gelir; IPermissionManagementDbContext bu dort seti talep eder.
    public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; } = default!;

    public DbSet<PermissionDefinitionRecord> Permissions { get; set; } = default!;

    public DbSet<PermissionGrant> PermissionGrants { get; set; } = default!;

    public DbSet<ResourcePermissionGrant> ResourcePermissionGrants { get; set; } = default!;

    // Feature tanimlari ve provider degerleri cerceveden gelir; IFeatureManagementDbContext bu uc seti talep eder.
    public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; } = default!;

    public DbSet<FeatureDefinitionRecord> Features { get; set; } = default!;

    public DbSet<FeatureValue> FeatureValues { get; set; } = default!;

    // Kiraci entity'leri cerceveden gelir; ITenantManagementDbContext bu iki seti talep eder.
    public DbSet<Tenant> Tenants { get; set; } = default!;

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; } = default!;

    public AuthManagementDbContext(DbContextOptions<AuthManagementDbContext> options)
        : base(options)
    {

    }

    // Devralinan konular kendi kurulum uzantilarini getirir; snake_case cevrimi en sona kalir cunku
    // butun adlar kuruldiktan sonra tek geciste cevrilir.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureIdentity();

        builder.ConfigureOpenIddict();

        builder.ConfigurePermissionManagement();

        builder.ConfigureFeatureManagement();

        builder.ConfigureTenantManagement();

        builder.ConfigureAuthManagement();

        builder.ApplySnakeCaseNaming();
    }
}
