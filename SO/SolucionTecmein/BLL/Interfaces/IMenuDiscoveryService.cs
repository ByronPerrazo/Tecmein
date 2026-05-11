using System.Reflection;

namespace BLL.Interfaces
{
    public interface IMenuDiscoveryService
    {
        Task<string> DiscoverAndRegisterMenusAsync(Assembly controllerAssembly);
    }
}
