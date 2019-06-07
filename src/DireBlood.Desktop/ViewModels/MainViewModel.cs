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


        private string _status;
        private ProxyDetailsModel _selectedProxy;
        private ObservableCollection<ProxyDetailsModel> _proxyViews = new ObservableCollection<ProxyDetailsModel>();

        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public string Title => Resources.Title;

        public ProxyDetailsModel SelectedProxy
        {
            get => _selectedProxy;
            set => Set(ref _selectedProxy, value);
        }

        public ObservableCollection<ProxyDetailsModel> ProxyViewModels
        {
            get => _proxyViews;
            set => Set(ref _proxyViews, value);
        }

        #region Commands

        public ICommand ShowGithubCommand { get; }
        public ICommand ShowInfoCommand { get; }
        public ICommand GetFromFileCommand { get; }
        public ICommand SaveToFileCommand { get; }

        private ICommand _checkAllProxyCommand;

        public ICommand CheckAllProxyCommand
        {
            get
            {
                return _checkAllProxyCommand ?? (_checkAllProxyCommand =
                           new RelayCommand(async () => await CheckProxyAsync(ProxyViewModels, proxyService, 100), o => !jobManager.IsRunning));
            }
        }

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
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding == false).ToArray(), proxyService, 100), o => !jobManager.IsRunning));
            }
        }

        private ICommand _checkRespondingProxyCommand;

        public ICommand CheckRespondingProxyCommand
        {
            get
            {
                return _checkRespondingProxyCommand ?? (_checkRespondingProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding).ToArray(), proxyService, 100), o => !jobManager.IsRunning));
            }
        }

        private ICommand _checkSelectedProxyCommand;

        public ICommand CheckSelectedProxyCommand
        {
            get
            {
                return _checkSelectedProxyCommand ?? (_checkSelectedProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(new List<ProxyDetailsModel> { SelectedProxy }, proxyService, 100), o => SelectedProxy != null && !jobManager.IsRunning));
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


        public MainViewModel(IJobManager jobManager, IProxyService proxyService, IDialogCoordinator dialogCoordinator)
        {
            this.jobManager = jobManager;
            this.proxyService = proxyService;

            ShowGithubCommand = new ShowGithubCommand(this, dialogCoordinator).GetCommand();
            ShowInfoCommand = new ShowInfoCommand(this, dialogCoordinator).GetCommand();
            GetFromFileCommand = new GetFromFileCommand(this).GetCommand();
            SaveToFileCommand = new SaveToFileCommand(this).GetCommand();
        }

        private async Task CheckProxyAsync(ICollection<ProxyDetailsModel> proxies, IProxyService proxyService, int threads)
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
                .OnProgressChanged(args => SetStatus($"Sprawdzam proxy.. T:{args.Count} G:{args.Good} B:{args.Bad} L:{args.Count - args.Bad - args.Good} {args.GetPergentage()}%"))
                .OnSuccess(args => SetStatus($"Zakończono sprawdzanie proxy.. T:{args.Count} G:{args.Good} B:{args.Bad}"));

            await jobManager.ExecuteAsync(job);
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
