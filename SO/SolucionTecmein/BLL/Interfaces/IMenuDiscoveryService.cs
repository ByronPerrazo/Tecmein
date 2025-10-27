using System.Reflection;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMenuDiscoveryService
    {
        Task<string> DiscoverAndRegisterMenusAsync(Assembly controllerAssembly);
    }
}
