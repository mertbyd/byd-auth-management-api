using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Byd.AuthManagement.Dtos.Organizations;
using Byd.AuthManagement.Interfaces.Organizations;
using Byd.AuthManagement.Managers.Organizations;
using Byd.AuthManagement.Mappers.Organizations;

namespace Byd.AuthManagement.Services.Organizations;

/// <summary>
/// Organizasyon agacinin okuma ve yazma orkestrasyonunu yurutur.
/// </summary>
// islevi: Okumalari repository projeksiyonundan, yazmalari Manager'dan gecirip cevap DTO'suna esler.
// sistemdeki gorevi: Controller ile organizasyon zinciri arasindaki tek public giris noktasidir.
// TODO: Tenant'li uclar kapsami route'tan aliyor; token'dan mi cozulecegi mentorlere sorulacak.
public class OrganizationAppService : AuthManagementAppService, IOrganizationAppService
{
    private static readonly OrganizationMapper Mapper = new();

    private OrganizationManager Manager =>
        LazyGetRequiredService<OrganizationManager>();
    private IOrganizationRepository Repository =>
        LazyGetRequiredService<IOrganizationRepository>();
    private IValidator<CreateOrganizationDto> CreateValidator =>
        LazyGetRequiredService<IValidator<CreateOrganizationDto>>();
    private IValidator<UpdateOrganizationDto> UpdateValidator =>
        LazyGetRequiredService<IValidator<UpdateOrganizationDto>>();

    // Kimligi verilen organizasyonu getirir.
    public virtual async Task<OrganizationDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Mapper.Map(await Repository.GetModelAsync(id, cancellationToken));

    // Ust organizasyonu olmayan kayitlari getirir.
    public virtual async Task<List<OrganizationDto>> GetRootsAsync(CancellationToken cancellationToken = default)
        => Mapper.Map(await Repository.GetChildModelListAsync(parentId: null, cancellationToken));

    // Kimligi verilen organizasyonun dogrudan alt kayitlarini getirir.
    public virtual async Task<List<OrganizationDto>> GetChildrenAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => Mapper.Map(await Repository.GetChildModelListAsync(id, cancellationToken));

    // Tenant altindaki tum organizasyonlari getirir.
    public virtual async Task<List<OrganizationDto>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        using var tenant = CurrentTenant.Change(tenantId);
        return Mapper.Map(await Repository.GetModelListAsync(cancellationToken));
    }

    // Tenant altindaki kok organizasyonlari getirir.
    public virtual async Task<List<OrganizationDto>> GetTenantRootsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        using var tenant = CurrentTenant.Change(tenantId);
        return Mapper.Map(await Repository.GetChildModelListAsync(parentId: null, cancellationToken));
    }

    // Tenant organizasyonlarini agac kodu sirasinda getirir.
    public virtual async Task<List<OrganizationDto>> GetTenantTreeAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        using var tenant = CurrentTenant.Change(tenantId);
        return Mapper.Map(await Repository.GetModelListAsync(cancellationToken));
    }

    // Kullanicinin tenant icindeki organizasyon uyeliklerini getirir.
    public virtual async Task<List<OrganizationDto>> GetUserOrganizationsAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var tenant = CurrentTenant.Change(tenantId);
        return Mapper.Map(await Repository.GetUserModelListAsync(userId, cancellationToken));
    }

    // Girdiyi dogrular, organizasyonu agaca ekler ve kaydedilmis halini dondurur.
    public virtual async Task<CreatedOrganizationDto> CreateAsync(
        CreateOrganizationDto input,
        CancellationToken cancellationToken = default)
    {
        await CreateValidator.ValidateAndThrowAsync(input, cancellationToken);
        var result = await Manager.CreateAsync(Mapper.Map(input), cancellationToken);
        await CurrentUnitOfWork!.SaveChangesAsync(cancellationToken);
        return Mapper.Map(result);
    }

    // Girdiyi dogrular, organizasyonu tasir ve adini gunceller; kaydedilmis halini dondurur.
    public virtual async Task<OrganizationDto> UpdateAsync(
        Guid id,
        UpdateOrganizationDto input,
        CancellationToken cancellationToken = default)
    {
        await UpdateValidator.ValidateAndThrowAsync(input, cancellationToken);
        await Manager.UpdateAsync(id, Mapper.Map(input), cancellationToken);
        await CurrentUnitOfWork!.SaveChangesAsync(cancellationToken);
        return Mapper.Map(await Repository.GetModelAsync(id, cancellationToken));
    }

    // Kaydi alt agaciyla, organizasyon rollerini ve izinlerini Manager kuraliyla siler.
    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await Manager.DeleteAsync(id, cancellationToken);

    // Kullaniciyi organizasyona uye yapar.
    public virtual async Task AddMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        => await Manager.AddMemberAsync(id, userId, cancellationToken);

    // Kullaniciyi organizasyondan cikarir.
    public virtual async Task RemoveMemberAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        => await Manager.RemoveMemberAsync(id, userId, cancellationToken);
}
