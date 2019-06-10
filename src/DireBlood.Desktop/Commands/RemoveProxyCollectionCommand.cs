using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DireBlood.Core.Abstractions;
using DireBlood.Core.Models;
using DireBlood.Core.Services;

namespace DireBlood.Commands
{
    public class RemoveProxyCollectionCommand : ICommandFactory
    {
        private readonly IJobService jobService;
        private readonly IStatusService statusService;

        public RemoveProxyCollectionCommand(IJobService jobService, IStatusService statusService)
        {
            this.jobService = jobService;
            this.statusService = statusService;
        }

        private IEnumerable<Proxy> proxies = new List<Proxy>();
        public ICommandFactory UseProxy(IEnumerable<Proxy> proxies)
        {
            this.proxies = proxies;
            return this;
        }
        public ICommand Get()
        {
            return new BasicRelayCommand(() =>
            {
                var count = proxies.Count();
//                ProxyViewModels = new ObservableCollection<ProxyDetailsModel>(ProxyViewModels.Where(proxy => proxy.WasVerified));
//                statusService.SetStatus($"Pomyślnie usunięto {count - ProxyViewModels.Count} adresy proxy.");

            }, () => !jobService.IsRunning);
        }
    }
}