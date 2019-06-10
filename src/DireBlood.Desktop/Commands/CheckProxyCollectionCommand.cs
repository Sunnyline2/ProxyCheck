using System;
using System.Collections.Generic;
using System.Windows.Input;
using DireBlood.Core.Abstractions;
using DireBlood.Core.Models;
using DireBlood.Core.Services;
using DireBlood.Properties;

namespace DireBlood.Commands
{
    public class CheckProxyCollectionCommand : ICommandFactory
    {
        private readonly IJobService jobService;
        private readonly IProxyCheckRunner proxyCheckRunner;
        
        public CheckProxyCollectionCommand(
            IJobService jobService, 
            IProxyCheckRunner proxyCheckRunner)
        {
            this.jobService = jobService;
            this.proxyCheckRunner = proxyCheckRunner;
        }

        private IEnumerable<Proxy> proxies = new List<Proxy>();

        public ICommandFactory UseProxy(IEnumerable<Proxy> proxies)
        {
            this.proxies = proxies;
            return this;
        }

        public ICommand Get()
        {
            return new BasicRelayCommand(async () => await proxyCheckRunner.RunAsync(proxies, Settings.Default.MaxThreads), () => !jobService.IsRunning);
        }
    }
}