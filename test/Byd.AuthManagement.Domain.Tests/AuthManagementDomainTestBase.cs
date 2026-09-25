using Volo.Abp.Modularity;

namespace Byd.AuthManagement;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class AuthManagementDomainTestBase<TStartupModule> : AuthManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
