using System.Collections.Generic;
using System.Threading.Tasks;

namespace DireBlood.Core.Services
{
    public interface IProxyCheckRunner
    {
        Task RunAsync(IEnumerable<Models.Proxy> proxies, int threads);
    }
}