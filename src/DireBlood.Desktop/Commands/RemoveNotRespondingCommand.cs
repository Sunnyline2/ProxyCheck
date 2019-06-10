using DireBlood.Core.Abstractions;
using DireBlood.Core.ObservableDataProviders;
using DireBlood.Core.Services;

namespace DireBlood.Commands
{
    public class RemoveNotRespondingCommand : ICommandFactory
    {
        private readonly IJobService jobService;
        private readonly IObservableDataProvider<ProxyDetailsModel> proxyRepository;
        private readonly IStatusService statusService;

        public RemoveNotRespondingCommand(IObservableDataProvider<ProxyDetailsModel> proxyRepository,
            IStatusService statusService, IJobService jobService)
        {
            this.proxyRepository = proxyRepository;
            this.statusService = statusService;
            this.jobService = jobService;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(() =>
            {
                var count = proxyRepository.GetAll().Count;
                proxyRepository.Set(proxyRepository.GetAll().Where(proxy => proxy.IsResponding));
                statusService.SetStatus($"Pomyślnie usunięto {count - proxyRepository.GetAll().Count} adresy proxy.");
            }, o => !jobService.IsRunning);
        }
    }
}