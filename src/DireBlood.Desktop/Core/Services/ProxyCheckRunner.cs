using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DireBlood.Core.Models;
using DireBlood.EventArgs;

namespace DireBlood.Core.Services
{
    public class ProxyCheckRunner : IProxyCheckRunner
    {
        private readonly IJobService jobService;
        private readonly IProxyService proxyService;
        private readonly IStatusService statusService;

        public ProxyCheckRunner(IJobService jobService, IProxyService proxyService, IStatusService statusService)
        {
            this.jobService = jobService;
            this.proxyService = proxyService;
            this.statusService = statusService;
        }

        public async Task RunAsync(IEnumerable<Proxy> proxies, int threads)
        {
            var job = new JobAsync<ProxyCheckingEventArgs>((progress, args) => Task.Run(async () =>
                {
                    args.Count = proxies.Count();
                    var semaphore = new SemaphoreSlim(threads);
                    var tasks = new List<Task>();
                    var sync = new object();

                    for (var i = 0; i < proxies.Count; i++)
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
                                var proxyInfo = await proxyService.CheckAsync(proxy.Host, proxy.Port,
                                    TimeSpan.FromSeconds(5), CancellationToken.None);

                                proxy.Update(proxyInfo, true);
                                lock (sync)
                                {
                                    if (proxyInfo.IsResponding)
                                        args.Good++;
                                    else
                                        args.Bad++;
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
                .OnProgressChanged(args =>
                    statusService.SetStatus(
                        $"Sprawdzam proxy.. T:{args.Count} G:{args.Good} B:{args.Bad} L:{args.Count - args.Bad - args.Good} {args.GetPercentage()}%"))
                .OnSuccess(args =>
                    statusService.SetStatus(
                        $"Zakończono sprawdzanie proxy.. T:{args.Count} G:{args.Good} B:{args.Bad}"));

            await jobService.ExecuteAsync(job);
        }

    }
}