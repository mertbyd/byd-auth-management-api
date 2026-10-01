using System;
using System.Collections.Generic;
using Byd.AuthManagement.Models.Logins;
using Byd.AuthManagement.Models.Organizations;
using Byd.AuthManagement.Models.Registrations;
using Byd.CoreExtensions.Scopes;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;

namespace Byd.AuthManagement.Extensions.Accounts;

/// <summary>
/// Hesap akışlarının domain modellerine yaptığı saf alan aktarımlarını merkezileştirir.
/// </summary>
// islevi: Manager'larin hesap, kapsam ve oturum modellerini tekrar eden alan listeleri olmadan kurmasini saglar.
// sistemdeki gorevi: Karar ve veri erisimi icermeyen domain model donusumlerinin tek yeridir.
public static class AccountModelExtensions
{
    // Yeni tenant'i kayit akisinin ortak kapsam modeline donusturur.
    public static RegistrationScopeModel ToRegistrationScopeModel(
        this Tenant tenant,
        List<string> roleNames)
        => CreateRegistrationScopeModel(tenant.Id, tenant.Name, roleNames);

    // Mevcut tenant'i kayit akisinin ortak kapsam modeline donusturur.
    public static RegistrationScopeModel ToRegistrationScopeModel(
        this TenantConfiguration tenant,
        List<string> roleNames)
        => CreateRegistrationScopeModel(tenant.Id, tenant.Name, roleNames);

    // Mevcut tenant ve organizasyonu kayit akisinin ortak kapsam modeline donusturur.
    public static RegistrationScopeModel ToRegistrationScopeModel(
        this TenantConfiguration tenant,
        OrganizationUnit organization,
        List<string> roleNames)
        => CreateRegistrationScopeModel(
            tenant.Id,
            tenant.Name,
            roleNames,
            organization.Id,
            organization.DisplayName);

    // Kayit kapsamini olusturulan kullaniciyla birlestirip public sonucu kurar.
    public static RegisteredUserModel ToRegisteredUserModel(
        this RegistrationScopeModel scope,
        IdentityUser user)
    {
        return new RegisteredUserModel
        {
            TenantId = scope.TenantId,
            TenantName = scope.TenantName,
            OrganizationId = scope.OrganizationId,
            OrganizationName = scope.OrganizationName,
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    // Kimlik rolunu hesaplanan seviyesiyle oturum rol modeline donusturur.
    public static CurrentLoginRoleModel ToCurrentLoginRoleModel(
        this IdentityRole role,
        ScopeLevel scopeLevel)
    {
        return new CurrentLoginRoleModel
        {
            Id = role.Id,
            Name = role.Name,
            ScopeLevel = scopeLevel
        };
    }

    // Gecerli kullanici ve kapsam degerlerini me cevabinin domain modelinde birlestirir.
    public static CurrentLoginModel ToCurrentLoginModel(
        this ICurrentUser currentUser,
        Guid userId,
        Guid? tenantId,
        string? tenantName,
        Guid? organizationId,
        string? organizationName,
        ScopeLevel scopeLevel,
        List<CurrentLoginRoleModel> roles,
        List<OrganizationModel> organizations)
    {
        return new CurrentLoginModel
        {
            TenantId = tenantId,
            TenantName = tenantName,
            OrganizationId = organizationId,
            OrganizationName = organizationName,
            UserId = userId,
            ScopeLevel = scopeLevel,
            UserName = currentUser.UserName!,
            Name = currentUser.Name,
            Surname = currentUser.SurName,
            Email = currentUser.Email,
            Roles = roles,
            Organizations = organizations
        };
    }

    // Tenant ve istege bagli organizasyon alanlarindan ortak kayit kapsam modelini kurar.
    private static RegistrationScopeModel CreateRegistrationScopeModel(
        Guid tenantId,
        string tenantName,
        List<string> roleNames,
        Guid? organizationId = null,
        string? organizationName = null)
    {
        return new RegistrationScopeModel
        {
            TenantId = tenantId,
            TenantName = tenantName,
            OrganizationId = organizationId,
            OrganizationName = organizationName,
            RoleNames = roleNames
        };
    }
}
