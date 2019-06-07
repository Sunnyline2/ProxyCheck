using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Core.Job;
using DireBlood.Models;
using DireBlood.Services;

namespace DireBlood.Commands
{
    public class CheckProxyCollectionCommand : ICommandFactory
    {
        private readonly IJobManager jobManager;
        private readonly IProxyCheckService proxyCheckService;
        private readonly IList<ProxyDetailsModel> proxyDetailsModels;

        public CheckProxyCollectionCommand(IJobManager jobManager, IProxyCheckService proxyCheckService, IList<ProxyDetailsModel> proxyDetailsModels)
        {
            this.jobManager = jobManager;
            this.proxyCheckService = proxyCheckService;
            this.proxyDetailsModels = proxyDetailsModels;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await proxyCheckService.CheckProxyAsync(proxyDetailsModels, 100), o => !jobManager.IsRunning);
        }
    }
}
