using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Dtos.Organizations;
using Byd.AuthManagement.Permissions;
using Byd.AuthManagement.Services.Organizations;
using SystemStandards.Results;

namespace Byd.AuthManagement.Controllers.Organizations;

/// <summary>
/// Organizasyon agaci islemlerini HTTP uzerinden sunar.
/// </summary>
// islevi: Route ve request binding metadatasini tasiyip her istegi tek AppService cagrisina yonlendirir.
// sistemdeki gorevi: Organizasyon dikey kesitinin ince HTTP giris noktasidir.
[ApiController]
[Route(OrganizationRoutes.Root)]
public class OrganizationController : AuthManagementController
{
    /// <summary>Organizasyon AppService'ini lazy cozer.</summary>
    private IOrganizationAppService AppService =>
        LazyGetRequiredService<IOrganizationAppService>();

    /// <summary>
    /// Kimligi verilen organizasyonu getirir.
    /// </summary>
    /// <param name="id">Getirilecek organizasyon kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value { Id, TenantId, ParentId, ParentName, Code, DisplayName, CreationTime, LastModificationTime }.
    /// Kok organizasyonda ParentId ve ParentName bos doner. Alt kayitlar bu endpointte donmez; onlar icin
    /// children endpointi kullanilir.
    /// </remarks>
    [HttpGet(OrganizationRoutes.ById)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<OrganizationDto>> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await AppService.GetAsync(id, cancellationToken);

    /// <summary>
    /// Ust organizasyonu olmayan kayitlari getirir.
    /// </summary>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value elemanlari tek kayit endpointiyle ayni alanlari tasir; ParentId ve ParentName bos gelir.
    /// Liste sayfalanmaz ve agacin yalnizca ilk seviyesini dondurur; kok yoksa bos liste doner.
    /// </remarks>
    [HttpGet(OrganizationRoutes.Roots)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<List<OrganizationDto>>> GetRootsAsync(
        CancellationToken cancellationToken = default)
        => await AppService.GetRootsAsync(cancellationToken);

    /// <summary>
    /// Kimligi verilen organizasyonun dogrudan alt kayitlarini getirir.
    /// </summary>
    /// <param name="id">Alt kayitlari getirilecek organizasyonun kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value elemanlari tek kayit endpointiyle ayni alanlari tasir. Yalnizca bir seviye asagi inilir;
    /// torunlar donmez ve alt kayit yoksa bos liste doner.
    /// </remarks>
    [HttpGet(OrganizationRoutes.Children)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<List<OrganizationDto>>> GetChildrenAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await AppService.GetChildrenAsync(id, cancellationToken);

    /// <summary>
    /// Tenant altindaki tum organizasyonlari getirir.
    /// </summary>
    /// <param name="tenantId">Organizasyonlari listelenecek tenant kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value tenant kapsamindaki tum organizasyonlari tek kayit endpointiyle ayni alanlarla, agac kodu
    /// sirasinda tasir. Liste sayfalanmaz; organizasyon yoksa bos liste doner.
    /// </remarks>
    [HttpGet(OrganizationRoutes.ByTenant)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<List<OrganizationDto>>> GetByTenantAsync(
        [FromRoute] Guid tenantId,
        CancellationToken cancellationToken = default)
        => await AppService.GetByTenantAsync(tenantId, cancellationToken);

    /// <summary>
    /// Tenant altindaki kok (parent'i olmayan) organizasyonlari getirir.
    /// </summary>
    /// <param name="tenantId">Kok organizasyonlari listelenecek tenant kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value yalnizca en ust seviye tenant organizasyonlarini tasir; ParentId ve ParentName bos gelir.
    /// </remarks>
    [HttpGet(OrganizationRoutes.TenantRoots)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<List<OrganizationDto>>> GetTenantRootsAsync(
        [FromRoute] Guid tenantId,
        CancellationToken cancellationToken = default)
        => await AppService.GetTenantRootsAsync(tenantId, cancellationToken);

    /// <summary>
    /// Tenant organizasyonlarini hiyerarsik agac kodu sirasinda getirir.
    /// </summary>
    /// <param name="tenantId">Agac yapisi listelenecek tenant kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value organizasyonlari hiyerarsik agac koduna gore sirali tasir; on yuzde agac render etmek icin uygundur.
    /// </remarks>
    [HttpGet(OrganizationRoutes.TenantTree)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<List<OrganizationDto>>> GetTenantTreeAsync(
        [FromRoute] Guid tenantId,
        CancellationToken cancellationToken = default)
        => await AppService.GetTenantTreeAsync(tenantId, cancellationToken);

    /// <summary>
    /// Kullanicinin belirtilen tenant icindeki organizasyon uyeliklerini getirir.
    /// </summary>
    /// <param name="tenantId">Uyeliklerin sorgulanacagi tenant kimligi.</param>
    /// <param name="userId">Organizasyonlari sorgulanacak kullanici kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Response value kullanicinin dahil oldugu organizasyonlari tasir. Kullanici bu tenant'ta yoksa veya hicbir
    /// organizasyona bagli degilse bos liste doner; ayrim yapilmaz.
    /// </remarks>
    [HttpGet(OrganizationRoutes.UserOrganizations)]
    [Authorize(AuthManagementPermissions.Organizations.View)]
    public virtual async Task<Result<List<OrganizationDto>>> GetUserOrganizationsAsync(
        [FromRoute] Guid tenantId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
        => await AppService.GetUserOrganizationsAsync(tenantId, userId, cancellationToken);

    /// <summary>
    /// Yeni bir organizasyon olusturur.
    /// </summary>
    /// <param name="input">Organizasyon, baglanacagi ust kayit, feature degerleri ve yonetici hesap bilgileri.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { ParentId, DisplayName, AdminUserName, AdminEmail, AdminPassword, Features[{ Name, Value }] }.
    /// Organizasyonla birlikte secilen feature degerleri kendi kapsaminda yazilir; boolean, sayisal ve secimli
    /// degerler feature taniminin kuraliyla dogrulanir. Organizasyonla birlikte kimliginden
    /// tureyen statik admin rolu acilir, bes yonetim izni verilir ve yonetici bu role dogrudan baglanip uye yapilir.
    /// E-posta mevcutsa hesap ve sifresi korunur. Response organizasyon alanlarina ek olarak AdminRoleId,
    /// AdminRoleName, AdminUserId ve IsAdminUserNew tasir. Organizasyon kapsamindaki yonetici yalniz kendi alt
    /// agacinda kayit acabilir ve ParentId vermek zorundadir.
    /// </remarks>
    [HttpPost]
    [Authorize(AuthManagementPermissions.Organizations.Manage)]
    [Authorize(AuthManagementPermissions.Features.Manage)]
    public virtual async Task<Result<CreatedOrganizationDto>> CreateAsync(
        [FromBody] CreateOrganizationDto input,
        CancellationToken cancellationToken = default)
        => await AppService.CreateAsync(input, cancellationToken);

    /// <summary>
    /// Kimligi verilen organizasyonun adini ve agactaki yerini gunceller.
    /// </summary>
    /// <param name="id">Guncellenecek organizasyon kaydinin kimligi.</param>
    /// <param name="input">Yeni organizasyon adi, baglanacagi ust organizasyon ve istege bagli feature degerleri.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Request { ParentId, DisplayName, Features[{ Name, Value }]? }. ParentId degisirse kayit tasinir ve alt agacin
    /// kodlari yeniden uretilir. Features gonderilirse organizasyonun feature degerleri bu listeyle esitlenir ve
    /// Features.Manage yetkisi gerekir; gonderilmezse feature'lara dokunulmaz. Response value guncel kodu tasir.
    /// Alt kayitlar bu endpointten duzenlenmez.
    /// </remarks>
    [HttpPut(OrganizationRoutes.ById)]
    [Authorize(AuthManagementPermissions.Organizations.Manage)]
    public virtual async Task<Result<OrganizationDto>> UpdateAsync(
        Guid id,
        [FromBody] UpdateOrganizationDto input,
        CancellationToken cancellationToken = default)
        => await AppService.UpdateAsync(id, input, cancellationToken);

    /// <summary>
    /// Kimligi verilen organizasyonu alt agaciyla birlikte siler.
    /// </summary>
    /// <param name="id">Silinecek organizasyon kaydinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Kayit ve tum alt kayitlari silinir; bagli roller, izin kayitlari, kullanici-rol baglari ve organizasyonlarin
    /// feature degerleri temizlenir.
    /// Kullanici hesaplari silinmez. Organizasyon yoneticisi kendi organizasyonunu silemez. Islem geri alinamaz.
    /// Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpDelete(OrganizationRoutes.ById)]
    [Authorize(AuthManagementPermissions.Organizations.Manage)]
    public virtual async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await AppService.DeleteAsync(id, cancellationToken);
        return Result.NoContent();
    }

    /// <summary>
    /// Kullaniciyi organizasyona uye yapar.
    /// </summary>
    /// <param name="id">Uye eklenecek organizasyonun kimligi.</param>
    /// <param name="userId">Uye yapilacak kullanicinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Istek govdesi yoktur. Organizasyon istegin tenant'inda olmalidir. Kullanici zaten uyeyse degisiklik yapilmaz.
    /// Organizasyonun rolleri kullanicinin bir sonraki login'inde ya da refresh'inde token'a gelir; mevcut token
    /// degismez. Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpPost(OrganizationRoutes.Member)]
    [Authorize(AuthManagementPermissions.Organizations.Manage)]
    public virtual async Task<Result> AddMemberAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await AppService.AddMemberAsync(id, userId, cancellationToken);
        return Result.NoContent();
    }

    /// <summary>
    /// Kullaniciyi organizasyondan cikarir.
    /// </summary>
    /// <param name="id">Uyeligi kaldirilacak organizasyonun kimligi.</param>
    /// <param name="userId">Organizasyondan cikarilacak kullanicinin kimligi.</param>
    /// <param name="cancellationToken">Istek iptal belirteci.</param>
    /// <remarks>
    /// Istek govdesi yoktur. Organizasyon istegin tenant'inda olmalidir. Kullanici uye degilse degisiklik yapilmaz.
    /// Organizasyonun rolleri kullanicinin bir sonraki login'inde ya da refresh'inde token'dan duser; mevcut token
    /// degismez. Basarili istekte iceriksiz cevap dondurulur.
    /// </remarks>
    [HttpDelete(OrganizationRoutes.Member)]
    [Authorize(AuthManagementPermissions.Organizations.Manage)]
    public virtual async Task<Result> RemoveMemberAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await AppService.RemoveMemberAsync(id, userId, cancellationToken);
        return Result.NoContent();
    }
}
