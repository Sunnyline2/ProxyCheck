using System;
using System.Threading;
using System.Threading.Tasks;
using CheckProxy.Core.Proxy;

namespace CheckProxy.Core
{
    public interface IProxyService
    {
        Task<ICheckProxyResult> CheckProxyAsync(string host, ushort port, TimeSpan timeout, CancellationToken cancellationToken);
    }
}