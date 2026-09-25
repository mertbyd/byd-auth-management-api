using Byd.AuthManagement.Localization;
using Volo.Abp.Application.Services;

namespace Byd.AuthManagement;

public abstract class AuthManagementAppService : ApplicationService
{
    protected AuthManagementAppService()
    {
        LocalizationResource = typeof(AuthManagementResource);
        ObjectMapperContext = typeof(AuthManagementApplicationModule);
    }
}
