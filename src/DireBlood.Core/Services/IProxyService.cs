using System;
using System.Threading;
using System.Threading.Tasks;
using DireBlood.Core.Proxy;

namespace DireBlood.Core.Services
{

    public interface IProxyService
    {
        Task<IProxyModel> CheckProxyAsync(string host, ushort port, TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}