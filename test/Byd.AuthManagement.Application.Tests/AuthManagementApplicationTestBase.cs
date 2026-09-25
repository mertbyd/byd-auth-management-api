using Volo.Abp.Modularity;

namespace Byd.AuthManagement;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class AuthManagementApplicationTestBase<TStartupModule> : AuthManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
