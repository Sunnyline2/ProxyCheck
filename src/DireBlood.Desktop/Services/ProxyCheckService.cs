using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DireBlood.Core.Job;
using DireBlood.Core.Services;
using DireBlood.EventArgs;
using DireBlood.Models;

namespace DireBlood.Services
{
    public interface IProxyCheckService
    {
        Task CheckProxyAsync(ICollection<ProxyDetailsModel> proxies, int threads);
    }

    public class ProxyCheckService : IProxyCheckService
    {
        private readonly IJobManager jobManager;
        private readonly IProxyService proxyService;
        private readonly IStatusService statusService;

        public ProxyCheckService(IJobManager jobManager, IProxyService proxyService, IStatusService statusService)
        {
            this.jobManager = jobManager;
            this.proxyService = proxyService;
            this.statusService = statusService;
        }

        public async Task CheckProxyAsync(ICollection<ProxyDetailsModel> proxies, int threads)
        {
            var job = new JobAsync<ProxyCheckingEventArgs>((progress, args) => Task.Run(async () =>
            {
                args.Count = proxies.Count;
                var semaphore = new SemaphoreSlim(threads);
                var tasks = new List<Task>();
                var sync = new object();

                for (int i = 0; i < proxies.Count; i++)
                {
                    await semaphore.WaitAsync();
                    args.Current = i;
                    progress.Report(args);

                    var proxy = proxies.ElementAt(i);
                    proxy.Status = ProxyDetailsModel.ProxyStatus.InProcess;

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var proxyInfo = await proxyService.CheckProxyAsync(proxy.Host, proxy.Port,
                                TimeSpan.FromSeconds(5), CancellationToken.None);

                            proxy.Update(proxyInfo, true);
                            lock (sync)
                            {
                                if (proxyInfo.IsResponding)
                                {
                                    args.Good++;
                                }
                                else
                                {
                                    args.Bad++;
                                }
                            }
                            proxy.Status = ProxyDetailsModel.ProxyStatus.Ready;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());
            }))
                .OnProgressChanged(args => statusService.SetStatus($"Sprawdzam proxy.. T:{args.Count} G:{args.Good} B:{args.Bad} L:{args.Count - args.Bad - args.Good} {args.GetPergentage()}%"))
                .OnSuccess(args => statusService.SetStatus($"Zakończono sprawdzanie proxy.. T:{args.Count} G:{args.Good} B:{args.Bad}"));

            await jobManager.ExecuteAsync(job);
        }
    }
}
