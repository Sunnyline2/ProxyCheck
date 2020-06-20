using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CheckProxy.Core;
using CheckProxy.Core.Job;
using CheckProxy.Core.Proxy;
using CheckProxy.Desktop.EventArgs;
using Microsoft.Win32;
using CheckProxy.Desktop.Models;
using CheckProxy.Desktop.Utilities;

namespace CheckProxy.Desktop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields

        private readonly JobManager _jobManager = new JobManager();
        private readonly IProxyService _proxyService = new ProxyService();
        private MetroWindow _window;
        private string _status;
        private string _title = $"ProxyCheck {Assembly.GetExecutingAssembly().GetName().Version}";
        private ProxyDetailsModel _selectedProxy;
        private ObservableCollection<ProxyDetailsModel> _proxyViews = new ObservableCollection<ProxyDetailsModel>();

        #endregion

        #region Properties

        public string Status
        {
            get => _status;
            set => Set(value, ref _status);
        }

        public string Title
        {
            get => _title;
            set => Set(value, ref _title);
        }

        public ProxyDetailsModel SelectedProxy
        {
            get => _selectedProxy;
            set => Set(value, ref _selectedProxy);
        }

        public MetroWindow Window => _window ?? (_window = Application.Current.MainWindow as MetroWindow);

        public ObservableCollection<ProxyDetailsModel> ProxyViewModels
        {
            get => _proxyViews;
            set => Set(value, ref _proxyViews);
        }

        #endregion

        #region Commands

        private ICommand _showGithubCommand;

        public ICommand ShowGithubCommand
        {
            get
            {
                return _showGithubCommand ??
                       (_showGithubCommand = new RelayCommand(async () => await ShowGithubInfoAsync()));
            }
        }

        private ICommand _showInfoCommand;

        public ICommand ShowInfoCommand
        {
            get { return _showInfoCommand ?? (_showInfoCommand = new RelayCommand(async () => await ShowInfoAsync())); }
        }

        private ICommand _getFromFileCommand;

        public ICommand GetFromFileCommand
        {
            get
            {
                return _getFromFileCommand ??
                       (_getFromFileCommand = new RelayCommand(async () => await GetFromFileAsync(), o => !_jobManager.IsRunning));
            }
        }

        private ICommand _saveToFileCommand;

        public ICommand SaveToFileCommand
        {
            get
            {
                return _saveToFileCommand ??
                       (_saveToFileCommand = new RelayCommand(async () => await SaveToFileAsync(), o => !_jobManager.IsRunning));
            }
        }

        private ICommand _checkAllProxyCommand;

        public ICommand CheckAllProxyCommand
        {
            get
            {
                return _checkAllProxyCommand ?? (_checkAllProxyCommand =
                           new RelayCommand(async () => await CheckProxyAsync(ProxyViewModels, _proxyService, 100), o => !_jobManager.IsRunning));
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
                       }, o => !_jobManager.IsRunning));
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
                }, o => !_jobManager.IsRunning));
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
                }, o => !_jobManager.IsRunning));
            }
        }

        private ICommand _checkNotRespondingProxyCommand;

        public ICommand CheckNotRespondingProxyCommand
        {
            get
            {
                return _checkNotRespondingProxyCommand ?? (_checkNotRespondingProxyCommand = new RelayCommand(async () =>
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding == false).ToArray(), _proxyService, 100), o => !_jobManager.IsRunning));
            }
        }

        private ICommand _checkRespondingProxyCommand;

        public ICommand CheckRespondingProxyCommand
        {
            get
            {
                return _checkRespondingProxyCommand ?? (_checkRespondingProxyCommand = new RelayCommand(async () => 
                       await CheckProxyAsync(ProxyViewModels.Where(proxy => proxy.IsResponding).ToArray(), _proxyService, 100), o => !_jobManager.IsRunning));
            }
        }

        private ICommand _checkSelectedProxyCommand;

        public ICommand CheckSelectedProxyCommand
        {
            get
            {
                return _checkSelectedProxyCommand ?? (_checkSelectedProxyCommand = new RelayCommand(async () => 
                       await CheckProxyAsync(new List<ProxyDetailsModel> { SelectedProxy }, _proxyService, 100), o => SelectedProxy != null && !_jobManager.IsRunning));
            }
        }

        private ICommand _removeSelectedProxyCommand;

        public ICommand RemoveSelectedProxyCommand
        {
            get
            {
                return _removeSelectedProxyCommand ?? (_removeSelectedProxyCommand = new RelayCommand(() => ProxyViewModels.Remove(SelectedProxy), o => SelectedProxy != null && !_jobManager.IsRunning));
            }
        }

        #endregion

        #region Constructors

#if DEBUG
        public MainViewModel() : this(default(MetroWindow))
        {

        }
#endif

        public MainViewModel(MetroWindow window = default(MetroWindow))
        {
            _window = window;

            var models = new List<ProxyDetailsModel>
            {
                new ProxyDetailsModel {Host = "35.162.160.108", Port = 8080},
                new ProxyDetailsModel {Host = "198.181.36.100", Port = 8080},
                new ProxyDetailsModel {Host = "54.152.198.150", Port = 3128},
                new ProxyDetailsModel {Host = "35.162.160.108", Port = 8080},
                new ProxyDetailsModel {Host = "35.162.160.108", Port = 8080},
            };

            foreach (var model in models)
            {
                ProxyViewModels.Add(model);
            }
        }

        #endregion

        #region Commands

        private async Task ShowGithubInfoAsync()
        {
            var dialogResult = await Window.ShowMessageAsync(Title, "Zostaniesz przeniesiony na mojego githuba.\n" +
                                                                    "Czy chcesz kontynuować?",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings { AffirmativeButtonText = "Tak", NegativeButtonText = "Nie" });

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                Process.Start("http://github.com/nickoff/");
            }
        }

        private async Task ShowInfoAsync()
        {
            await Window.ShowMessageAsync(Title, "Icons made by Freepik from www.flaticon.com is licensed by CC 3.0 BY");
        }

        private async Task GetFromFileAsync()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Wybierz plik",
                CheckFileExists = true,
                Filter = "Pliki tekstowe (.txt)|*.txt|Wszystkie pliki|*.*",
                Multiselect = true,
            };

            if (fileDialog.ShowDialog(Window) == false)
                return;

            var job = new JobAsync<FileReadingEventArgs>((progress, args) => Task.Run(async () =>
                {
                    using (var fileStream = new FileStream(fileDialog.FileName, FileMode.Open))
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        args.Count = streamReader.CountLines();
                        progress.Report(args);

                        var collection = new ObservableCollection<ProxyDetailsModel>(ProxyViewModels);

                        for (int index = 0; !streamReader.EndOfStream; index++)
                        {
                            args.Current = index;
                            var line = await streamReader.ReadLineAsync();
                            var match = RegexInstances.ProxyRegex.Value.Match(line);
                            if (match.Success)
                            {
                                var proxyView = new ProxyDetailsModel
                                {
                                    Host = match.Groups[1].Value,
                                    Port = ushort.Parse(match.Groups[2].Value),
                                };
                                collection.Add(proxyView);
                            }

                            progress.Report(args);
                        }

                        ProxyViewModels = collection;
                    }
                }))
                .OnProgressChanged(args => SetStatus($"Wczytywanie.. {args.Current}/{args.Count} {args.GetPergentage()}%"))
                .OnSuccess(args => SetStatus($"Pomyślnie wczytano {args.Count} adresów proxy."))
                .OnException(exception => SetStatus($"Wystąpił problem podczas wczytywania listy proxy. {exception.Message}"));

            await _jobManager.ExecuteAsync(job);
        }

        private async Task SaveToFileAsync()
        {
            if (ProxyViewModels.Any() == false)
                return;

            var saveDialog = new SaveFileDialog
            {
                Title = "Gdzie mam zapisać proxy?",
                DefaultExt = "plik tekstowy (.txt)|*.txt",
                OverwritePrompt = true,
            };

            var dialogResult = saveDialog.ShowDialog(Window);
            if (dialogResult == false)
                return;

            var job = new JobAsync<FileWritingEventArgs>((progress, args) => Task.Run(async () =>
                {
                    args.Count = ProxyViewModels.Count;
                    progress.Report(args);

                    using (var fileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        for (int i = 0; i < ProxyViewModels.Count; i++)
                        {
                            var proxy = ProxyViewModels.ElementAt(i);
                            await streamWriter.WriteLineAsync($"{proxy.Host}:{proxy.Port}");
                        }
                    }
                }))
                .OnProgressChanged(args => SetStatus($"Trwa zapisywanie.. {args.Current}/{args.Count} {args.GetPergentage()}%"))
                .OnSuccess(args => SetStatus($"Pomyślnie zapisano {args.Count} adresów proxy."));

            await _jobManager.ExecuteAsync(job);
        }


        private async Task BlockJobExecute()
        {
            await Window.ShowMessageAsync(Title, "Aktualnie wykonywanie jest inne zadanie.\n" +
                                                 "Poczekaj na jego zakończenie.");
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

            await _jobManager.ExecuteAsync(job);
        }

        #endregion

        #region Helpers


        #endregion
    }
}
