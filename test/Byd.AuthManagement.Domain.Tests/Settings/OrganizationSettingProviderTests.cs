using System;
using System.Threading.Tasks;
using NSubstitute;
using Byd.AuthManagement.Consts;
using Byd.AuthManagement.Providers.Settings;
using Byd.CoreExtensions.Interfaces;
using Shouldly;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Xunit;

namespace Byd.AuthManagement.Settings;

// islevi: Organizasyon ayar provider'larinin O provider'i ve organizasyon anahtariyla okuyup yazdigini dogrular.
// sistemdeki gorevi: Organizasyon degeri yoksa zincirin tenant ve global degerlere gectigini korur.
public class OrganizationSettingProviderTests
{
    private static readonly SettingDefinition Setting = new("Realtime.Fetch.VehiclePositionsIntervalSeconds", "10");

    [Fact]
    public async Task ValueProvider_ReadsOrganizationValue_WhenOrganizationIsSelected()
    {
        var organizationId = Guid.NewGuid();
        var store = Substitute.For<ISettingStore>();
        store.GetOrNullAsync(Setting.Name, SettingProviderConsts.OrganizationProviderName, organizationId.ToString())
            .Returns("5");
        var organization = Substitute.For<ICurrentOrganization>();
        organization.Id.Returns(organizationId);

        (await new OrganizationSettingValueProvider(store, organization).GetOrNullAsync(Setting)).ShouldBe("5");
    }

    [Fact]
    public async Task ValueProvider_PassesToNextProvider_WhenNoOrganizationIsSelected()
    {
        var store = Substitute.For<ISettingStore>();
        var organization = Substitute.For<ICurrentOrganization>();
        organization.Id.Returns((Guid?)null);

        (await new OrganizationSettingValueProvider(store, organization).GetOrNullAsync(Setting)).ShouldBeNull();
        await store.DidNotReceiveWithAnyArgs().GetOrNullAsync(default!, default, default);
    }

    [Fact]
    public async Task ManagementProvider_WritesToCurrentOrganization_WhenKeyIsOmitted()
    {
        var organizationId = Guid.NewGuid();
        var store = Substitute.For<ISettingManagementStore>();
        var organization = Substitute.For<ICurrentOrganization>();
        organization.Id.Returns(organizationId);

        await new OrganizationSettingManagementProvider(store, organization).SetAsync(Setting, "5", null);

        await store.Received(1).SetAsync(Setting.Name, "5", SettingProviderConsts.OrganizationProviderName,
            organizationId.ToString());
    }
}
