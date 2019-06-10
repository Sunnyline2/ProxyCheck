using DireBlood.Core.Abstractions;
using DireBlood.Core.ObservableDataProviders;
using DireBlood.Core.Services;

namespace DireBlood.Commands
{
    public class RemoveDuplicationsCommand : ICommandFactory
    {
        private readonly IJobService jobService;
        private readonly IObservableDataProvider<ProxyDetailsModel> proxyRepository;
        private readonly IStatusService statusService;

        public RemoveDuplicationsCommand(IObservableDataProvider<ProxyDetailsModel> proxyRepository,
            IJobService jobService, IStatusService statusService)
        {
            this.proxyRepository = proxyRepository;
            this.jobService = jobService;
            this.statusService = statusService;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(() =>
            {
                var count = proxyRepository.GetAll().Count;

                proxyRepository.Set(proxyRepository.GetAll().Distinct());
                statusService.SetStatus($"Pomyślnie usunięto {count - proxyRepository.GetAll().Count} adresy proxy.");
            }, o => !jobService.IsRunning);
        }
    }
}