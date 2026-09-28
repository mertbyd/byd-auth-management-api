using Byd.AuthManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;

namespace Byd.AuthManagement;

public abstract class AuthManagementController : AbpControllerBase
{
    protected AuthManagementController()
    {
        LocalizationResource = typeof(AuthManagementResource);
    }

    // ABP lazy provider uzerinden controller servislerini repository-native sekilde cozer.
    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return LazyServiceProvider.LazyGetRequiredService<TService>();
    }
}
