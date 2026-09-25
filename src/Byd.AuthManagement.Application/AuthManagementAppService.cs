using Byd.AuthManagement.Localization;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Byd.AuthManagement;

public abstract class AuthManagementAppService : ApplicationService
{
    protected AuthManagementAppService()
    {
        LocalizationResource = typeof(AuthManagementResource);
        ObjectMapperContext = typeof(AuthManagementApplicationModule);
    }

    // ABP lazy provider uzerinden application bagimliliklarini repository-native sekilde cozer.
    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return LazyServiceProvider.LazyGetRequiredService<TService>();
    }
}
