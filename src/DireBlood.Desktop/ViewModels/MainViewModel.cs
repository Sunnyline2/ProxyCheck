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
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;

namespace DireBlood.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public readonly JobManager JobManager = new JobManager();
        private readonly IProxyService _proxyService = new ProxyService();
        private MetroWindow _window;
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

        public MetroWindow Window => _window ?? (_window = Application.Current.MainWindow as MetroWindow);

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
                           new RelayCommand(async () => await CheckProxyAsync(ProxyViewModels, _proxyService, 100), o => !JobManager.IsRunning));
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
                       }, o => !JobManager.IsRunning));
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
                }, o => !JobManager.IsRunning));
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
                    ProxyViewModels = new ObservableCollection<ProxyDetailsModel>(ProxyViewModels.Where(proxy => proxy.WasVeryfied));
                    SetStatus($"Pomyślnie usunięto {count - ProxyViewModels.Count} adresy proxy.");
                }, o => !JobManager.IsRunning));
            }
        }

        private ICommand _checkNotRespondingProxyCommand;

        public ICommand CheckNotRespondingProxyCommand
        {
            get
            {
                return _checkNotRespondingProxyCommand ?? (_checkNotRespondingProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding == false).ToArray(), _proxyService, 100), o => !JobManager.IsRunning));
            }
        }

        private ICommand _checkRespondingProxyCommand;

        public ICommand CheckRespondingProxyCommand
        {
            get
            {
                return _checkRespondingProxyCommand ?? (_checkRespondingProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding).ToArray(), _proxyService, 100), o => !JobManager.IsRunning));
            }
        }

        private ICommand _checkSelectedProxyCommand;

        public ICommand CheckSelectedProxyCommand
        {
            get
            {
                return _checkSelectedProxyCommand ?? (_checkSelectedProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(new List<ProxyDetailsModel> { SelectedProxy }, _proxyService, 100), o => SelectedProxy != null && !JobManager.IsRunning));
            }
        }

        private ICommand _removeSelectedProxyCommand;

        public ICommand RemoveSelectedProxyCommand
        {
            get
            {
                return _removeSelectedProxyCommand ?? (_removeSelectedProxyCommand = new RelayCommand(() => ProxyViewModels.Remove(SelectedProxy), o => SelectedProxy != null && !JobManager.IsRunning));
            }
        }

        #endregion


        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                var models = new List<ProxyDetailsModel>
                {
                    new ProxyDetailsModel {Host = "35.162.160.108", Port = 8080},
                    new ProxyDetailsModel {Host = "198.181.36.100", Port = 8080},
                    new ProxyDetailsModel {Host = "54.152.198.150", Port = 3128},
                    new ProxyDetailsModel {Host = "35.162.160.108", Port = 8080},
                    new ProxyDetailsModel {Host = "35.162.160.108", Port = 8080}
                };

                foreach (var model in models)
                {
                    ProxyViewModels.Add(model);
                }
            }

            ShowGithubCommand = new ShowGithubCommand(Window).GetCommand();
            ShowInfoCommand = new ShowInfoCommand(Window).GetCommand();
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

            await JobManager.ExecuteAsync(job);
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
