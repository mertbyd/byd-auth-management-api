using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Byd.AuthManagement.Consts;
using Byd.CoreExtensions.Consts;
using Byd.CoreExtensions.Interfaces;
using Byd.CoreExtensions.Scopes;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Volo.Abp.Validation.StringValues;

namespace Byd.AuthManagement.Features;

/// <summary>
/// Host oturumunda feature erisimini acar, diger kapsamlarda ABP kontrolunu surdurur.
/// </summary>
// islevi: Host icin feature erisim istisnasini cercevenin sanal metotlarinda uygular.
// sistemdeki gorevi: IFeatureChecker, RequiresFeature ve izin kontrollerinin ortak ABP zincirini korur.
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IFeatureChecker), typeof(FeatureChecker), typeof(HostFeatureChecker))]
public class HostFeatureChecker(
    IOptions<AbpFeatureOptions> options,
    IServiceProvider serviceProvider,
    IFeatureDefinitionManager featureDefinitionManager,
    IFeatureValueProviderManager featureValueProviderManager,
    ICurrentUser currentUser,
    ICurrentTenant currentTenant,
    ICurrentOrganization currentOrganization)
    : FeatureChecker(options, serviceProvider, featureDefinitionManager, featureValueProviderManager)
{
    // Yalniz dogrulanmis host oturumunda ve host context'inde erisim istisnasi uygular.
    protected virtual bool IsHostScope =>
        currentUser.IsAuthenticated &&
        currentUser.FindClaim(ScopeClaimTypes.Level)?.Value == nameof(ScopeLevel.SuperAdmin) &&
        currentTenant.Id is null && currentOrganization.Id is null;
    // Host erisim kontrollerini acik kabul eder; diger oturumlari standart checker'a birakir.
    public override Task<bool> IsEnabledAsync(string name)
        => IsHostScope ? Task.FromResult(true) : base.IsEnabledAsync(name);
    // Host boolean degerlerini acik gosterirken sayisal ve secimli degerleri provider zincirinde korur.
    protected override Task<string?> GetOrNullValueFromProvidersAsync(
        IEnumerable<IFeatureValueProvider> providers,
        FeatureDefinition feature)
    {
        if (IsHostScope && IsBooleanFeature(feature))
            return Task.FromResult<string?>(FeatureConsts.EnabledValue);
        return base.GetOrNullValueFromProvidersAsync(providers, feature);
    }
    // Acik toggle tipini ve tipsiz boolean tanimlarini deger tasiyan diger feature'lardan ayirir.
    private static bool IsBooleanFeature(FeatureDefinition feature)
        => feature.ValueType is ToggleStringValueType ||
           feature.ValueType is null &&
           (feature.DefaultValue is null || bool.TryParse(feature.DefaultValue, out _));
}
