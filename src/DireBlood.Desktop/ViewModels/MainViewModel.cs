using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DireBlood.Commands;
using DireBlood.Commands.Abstractions;
using DireBlood.Core.Job;
using DireBlood.Core.Services;
using DireBlood.EventArgs;
using DireBlood.Models;
using DireBlood.Properties;
using DireBlood.Repository;
using DireBlood.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood.ViewModels
{


    public class MainViewModel : ViewModelBase
    {
        private readonly IJobManager jobManager;
        private readonly IProxyService proxyService;
        private readonly IStatusService statusService;


        private string status;
        private ProxyDetailsModel selectedProxy;
        private ObservableCollection<ProxyDetailsModel> proxyViews = new ObservableCollection<ProxyDetailsModel>();

        public string Status
        {
            get => status;
            set => Set(ref status, value);
        }

        public string Title => Resources.Title;

        public ProxyDetailsModel SelectedProxy
        {
            get => selectedProxy;
            set => Set(ref selectedProxy, value);
        }

        public ObservableCollection<ProxyDetailsModel> ProxyViewModels
        {
            get => proxyViews;
            set => Set(ref proxyViews, value);
        }

        #region Commands

        public ICommand ShowGithubCommand { get; }
        public ICommand ShowInfoCommand { get; }
        public ICommand GetFromFileCommand { get; }
        public ICommand SaveToFileCommand { get; }
        public ICommand CheckAllProxyCommand { get; }

        private ICommand _removeDuplicationsCommand;

        public ICommand RemoveDuplicationsCommand
        {
            get
            {
                return _removeDuplicationsCommand ??
                       (_removeDuplicationsCommand = new RelayCommand(() =>
                       {
                           var count = ProxyViewModels.Count;
                           ProxyViewModels = new ObservableCollection<ProxyDetailsModel>(ProxyViewModels.Distinct());
                           SetStatus($"Pomyślnie usunięto {count - ProxyViewModels.Count} adresy proxy.");
                       }, o => !jobManager.IsRunning));
            }
        }

        private ICommand _removeNotRespondingCommand;

        public ICommand RemoveNotRespondingCommand
        {
            get
            {
                return _removeNotRespondingCommand ?? (_removeNotRespondingCommand = new RelayCommand(() =>
                {
                    var count = ProxyViewModels.Count;
                    ProxyViewModels = new ObservableCollection<ProxyDetailsModel>(ProxyViewModels.Where(proxy => proxy.IsResponding));
                    SetStatus($"Pomyślnie usunięto {count - ProxyViewModels.Count} adresy proxy.");
                }, o => !jobManager.IsRunning));
            }
        }

        private ICommand _removeNotCheckedCommand;

        public ICommand RemoveNotCheckedCommand
        {
            get
            {
                return _removeNotCheckedCommand ?? (_removeNotCheckedCommand = new RelayCommand(() =>
                {
                    var count = ProxyViewModels.Count;
                    ProxyViewModels = new ObservableCollection<ProxyDetailsModel>(ProxyViewModels.Where(proxy => proxy.WasVerified));
                    SetStatus($"Pomyślnie usunięto {count - ProxyViewModels.Count} adresy proxy.");
                }, o => !jobManager.IsRunning));
            }
        }

        private ICommand _checkNotRespondingProxyCommand;

        public ICommand CheckNotRespondingProxyCommand
        {
            get
            {
                return _checkNotRespondingProxyCommand ?? (_checkNotRespondingProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding == false).ToArray(), 100), o => !jobManager.IsRunning));
            }
        }

        private ICommand _checkRespondingProxyCommand;

        public ICommand CheckRespondingProxyCommand
        {
            get
            {
                return _checkRespondingProxyCommand ?? (_checkRespondingProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding).ToArray(), 100), o => !jobManager.IsRunning));
            }
        }

        private ICommand _checkSelectedProxyCommand;

        public ICommand CheckSelectedProxyCommand
        {
            get
            {
                return _checkSelectedProxyCommand ?? (_checkSelectedProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(new List<ProxyDetailsModel> { SelectedProxy }, 100), o => SelectedProxy != null && !jobManager.IsRunning));
            }
        }

        private ICommand _removeSelectedProxyCommand;

        public ICommand RemoveSelectedProxyCommand
        {
            get
            {
                return _removeSelectedProxyCommand ?? (_removeSelectedProxyCommand = new RelayCommand(() => ProxyViewModels.Remove(SelectedProxy), o => SelectedProxy != null && !jobManager.IsRunning));
            }
        }

        #endregion


        public MainViewModel(IJobManager jobManager, IProxyService proxyService, IStatusService statusService,
            IDialogCoordinator dialogCoordinator, IObservableRepository<ProxyDetailsModel> proxyRepository, IProxyCheckService proxyCheckService)
        {
            this.jobManager = jobManager;
            this.proxyService = proxyService;
            this.statusService = statusService;

            this.statusService.OnStatusChanged += status => Status = status;

            this.ProxyViewModels = proxyRepository.GetAll();

            ShowGithubCommand = new ShowGithubCommand(this, dialogCoordinator).GetCommand();
            ShowInfoCommand = new ShowInfoCommand(this, dialogCoordinator).GetCommand();
            GetFromFileCommand = new GetFromFileCommand(this, jobManager, proxyRepository, statusService).GetCommand();
            SaveToFileCommand = new SaveToFileCommand(this, jobManager, statusService, proxyRepository).GetCommand();
            CheckAllProxyCommand = new CheckProxyCollectionCommand(jobManager, proxyCheckService, ProxyViewModels).GetCommand();
        }




        #region Helpers

        public void SetStatus(string value) => SafeInvoke(() => Status = value);

        public void SafeInvoke(Action action)
        {
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, action);
                }
            }
        }

        #endregion
    }
}
