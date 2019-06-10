using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DireBlood.Commands;
using DireBlood.Core.Models;
using DireBlood.Core.ObservableDataProviders;
using DireBlood.Core.Services;
using DireBlood.Properties;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IObservableDataProvider<Proxy> proxyObservableDataProvider;

        private Proxy selectedProxy;

        private string status;

        public MainViewModel(IJobService jobService, IStatusService statusService, IDialogCoordinator dialogCoordinator,
            IObservableDataProvider<ProxyDetailsModel> proxyRepository, IProxyCheckRunner proxyCheckRunner)
        {
            statusService.OnStatusChanged += status => Status = status;

            ShowGithubCommand = new ShowGithubCommand(this, dialogCoordinator).Get();
            ShowInfoCommand = new ShowInfoCommand(this, dialogCoordinator).Get();
            GetFromFileCommand = new GetFromFileCommand(this, jobService, proxyRepository, statusService).GetCommand();
            SaveToFileCommand = new SaveToFileCommand(this, jobService, statusService, proxyRepository).GetCommand();
            CheckAllProxyCommand = new CheckProxyCollectionCommand(jobService, proxyCheckRunner, ProxyViewModels)
                .Get();
            RemoveDuplicationsCommand =
                new RemoveDuplicationsCommand(proxyRepository, jobService, statusService).GetCommand();
            CheckNotRespondingProxyCommand = new CheckProxyCollectionCommand(jobService, proxyCheckRunner,
                ProxyViewModels.Where(x => !x.IsResponding).ToArray()).Get();
            CheckRespondingProxyCommand = new CheckProxyCollectionCommand(jobService, proxyCheckRunner,
                ProxyViewModels.Where(x => x.IsResponding).ToArray()).Get();
        }

        public string Status
        {
            get => status;
            set => Set(ref status, value);
        }

        public string Title => Resources.Title;

        public Proxy SelectedProxy
        {
            get => selectedProxy;
            set => Set(ref selectedProxy, value);
        }

        public ObservableCollection<Proxy> ProxyViewModels => proxyObservableDataProvider.Get();

        public ICommand ShowGithubCommand { get; }
        public ICommand ShowInfoCommand { get; }
        public ICommand GetFromFileCommand { get; }
        public ICommand SaveToFileCommand { get; }
        public ICommand CheckAllProxyCommand { get; }
        public ICommand RemoveDuplicationsCommand { get; }
        public ICommand RemoveNotRespondingCommand { get; }
        public ICommand RemoveNotCheckedCommand { get; }
        public ICommand CheckNotRespondingProxyCommand { get; }
        public ICommand CheckRespondingProxyCommand { get; }
        public ICommand CheckSelectedProxyCommand { get; }
        public ICommand RemoveSelectedProxyCommand { get; }
    }
}