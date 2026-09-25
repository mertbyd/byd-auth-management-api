using Byd.AuthManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Byd.AuthManagement;

public abstract class AuthManagementController : AbpControllerBase
{
    protected AuthManagementController()
    {
        LocalizationResource = typeof(AuthManagementResource);
    }
}
