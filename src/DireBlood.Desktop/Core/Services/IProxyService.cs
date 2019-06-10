using System;
using System.Threading;
using System.Threading.Tasks;

namespace DireBlood.Core.Services
{
    public interface IProxyService
    {
        Task<IProxyCheckResult> CheckAsync(string host, ushort port, TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}