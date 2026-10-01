using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.ExceptionCodes;
using Byd.AuthManagement.Models.Features;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.Validation;

namespace Byd.AuthManagement.Managers.Features;

// islevi: Secilen feature degerlerini okur, dogrular ve ABP yonetim zinciriyle uygular.
// sistemdeki gorevi: Tenant ve organizasyon atamalarinin ortak domain sahibidir.
public class FeatureSelectionManager(
    IFeatureManager featureManager,
    IFeatureDefinitionManager definitionManager) : DomainService
{
    // Ek alanlardaki secimi okur; secim varsa liste bicimini ve tanimlara uygunlugu denetler, yoksa null doner.
    public virtual async Task<List<FeatureSelectionModel>?> ReadFeaturesAsync(
        ExtraPropertyDictionary extraProperties,
        CancellationToken cancellationToken = default)
    {
        if (!extraProperties.TryGetValue(FeatureConsts.SelectionPropertyName, out var value) || value is null)
            return null;
        var features = DeserializeFeatures(value);
        ValidateSelection(features);
        await ValidateAsync(features, cancellationToken);
        return features;
    }

    // Yazmadan once tum adlari ve deger tiplerini tanimlarla karsilastirir.
    public virtual async Task ValidateAsync(
        IReadOnlyList<FeatureSelectionModel>? features,
        CancellationToken cancellationToken = default)
    {
        if (features is null || features.Count == 0)
            return;
        var definitions = (await definitionManager.GetAllAsync()).ToDictionary(definition => definition.Name);
        foreach (var feature in features)
        {
            if (!definitions.TryGetValue(feature.Name, out var definition))
                throw new BusinessException(FeatureExceptionCodes.Undefined);
            if (definition.ValueType is not null && !definition.ValueType.Validator.IsValid(feature.Value))
                throw new BusinessException(FeatureExceptionCodes.ValueInvalid);
        }
    }

    // Secim gonderilmediyse dokunmaz; gonderildiyse acik override listesini esitleyerek fallback'i korur.
    public virtual async Task ReplaceAsync(
        string providerName,
        string providerKey,
        IReadOnlyList<FeatureSelectionModel>? features,
        CancellationToken cancellationToken = default)
    {
        if (features is null)
            return;
        await ValidateAsync(features, cancellationToken);
        await featureManager.DeleteAsync(providerName, providerKey);
        foreach (var feature in features)
            await featureManager.SetAsync(feature.Name, feature.Value, providerName, providerKey, forceToSet: true);
    }

    // Ek alan JSON olarak gelir; ad-deger listesine cevrilemeyen secimi dogrulama hatasi olarak bildirir.
    private static List<FeatureSelectionModel> DeserializeFeatures(object value)
    {
        try
        {
            return JsonSerializer.SerializeToElement(value)
                       .Deserialize<List<FeatureSelectionModel>>(JsonSerializerOptions.Web)
                   ?? throw CreateInvalidSelectionException();
        }
        catch (JsonException)
        {
            throw CreateInvalidSelectionException();
        }
    }

    // Bos elemanlari, bos alanlari ve tekrar eden adlari ABP store'a ulasmadan reddeder.
    private static void ValidateSelection(IReadOnlyList<FeatureSelectionModel> features)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var feature in features)
        {
            if (feature is null)
                throw CreateInvalidSelectionException();
            if (string.IsNullOrWhiteSpace(feature.Name))
                throw CreateInvalidSelectionException(FeatureExceptionCodes.NameRequired);
            if (string.IsNullOrWhiteSpace(feature.Value))
                throw CreateInvalidSelectionException(FeatureExceptionCodes.ValueRequired);
            if (!names.Add(feature.Name))
                throw CreateInvalidSelectionException(FeatureExceptionCodes.NameDuplicated);
        }
    }

    // Okuma ve liste hatalarini ek alanin adiyla ayni public dogrulama sozlesmesinde bildirir.
    private static AbpValidationException CreateInvalidSelectionException(
        string errorCode = FeatureExceptionCodes.SelectionInvalid)
        => new(errorCode, [new ValidationResult(errorCode, [FeatureConsts.SelectionPropertyName])]);
}
