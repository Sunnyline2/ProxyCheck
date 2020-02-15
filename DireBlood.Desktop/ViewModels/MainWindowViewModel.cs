using CheckProxy.Core;
using CheckProxy.Core.Proxy;
using DireBlood.Desktop.Views;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DireBlood.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "DireBlood";

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<Proxy> Proxies { get; set; } = new ObservableCollection<Proxy>();

        private ObservableCollection<Proxy> choosedProxy = new ObservableCollection<Proxy>();

        public ObservableCollection<Proxy> ChoosedProxy
        {
            get { return choosedProxy; }
            set { SetProperty(ref choosedProxy, value); }
        }

        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();

        private readonly MetroWindow metroWindow;

        public MainWindowViewModel()
        {
            metroWindow = Application.Current.Windows
                .OfType<MetroWindow>()
                .FirstOrDefault();

            LoadFromFileCommand = new DelegateCommand(LoadFromFile);
            SaveToFileCommand = new DelegateCommand(SaveToFile);
            CheckAllProxyCommand = new DelegateCommand(async () => CheckAll());


            Proxies.Add(new Proxy("127.0.0.1", 80));
        }

        public ICommand LoadFromFileCommand { get; }

        public ICommand SaveToFileCommand { get; }

        public ICommand CheckAllProxyCommand { get; }

        private void LoadFromFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Wybierz plik",
                CheckFileExists = true,
                Filter = "Pliki tekstowe (.txt)|*.txt|Wszystkie pliki|*.*",
                Multiselect = true,
            };

            if (fileDialog.ShowDialog(metroWindow) == false)
                return;


            ThreadPool.QueueUserWorkItem(async delegate
            {
                var collection = new List<Proxy>(Proxies);

                using (var fileStream = new FileStream(fileDialog.FileName, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                {
                    //args.Count = streamReader.CountLines();
                    //progress.Report(args);

                    for (int index = 0; !streamReader.EndOfStream; index++)
                    {

                        var line = await streamReader.ReadLineAsync();
                        var match = RegexInstances.ProxyRegex.Value.Match(line);
                        if (!match.Success) continue;

                        var matchGroupHost = match.Groups[1].Value;
                        var matchGroupPort = match.Groups[2].Value;

                        if (!string.IsNullOrEmpty(matchGroupHost) &&
                            ushort.TryParse(matchGroupPort, out var port))
                        {
                            collection.Add(new Proxy(matchGroupHost, port));
                        }



                        //args.Current = index;

                        //progress.Report(args);
                    }

                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Proxies = new ObservableCollection<Proxy>(collection);
                    RaisePropertyChanged(nameof(Proxies));
                });
            });
        }

        private void SaveToFile()
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Wybierz gdzie zapisać plik",
                Filter = "Pliki tekstowe (.txt)|*.txt"
            };

            if (saveDialog.ShowDialog(metroWindow) == false)
                return;

            var content = string.Join(Environment.NewLine, Proxies.Select(proxy => proxy.ToString()).ToArray());
            File.WriteAllText(saveDialog.FileName, content);
        }

        private void CheckAll()
        {
            var configuration = new ProxyServiceConfiguration { Timeout = TimeSpan.FromSeconds(5) };
            var proxyService = new ProxyService(configuration);

            ThreadPool.QueueUserWorkItem(delegate
            {
                Parallel.ForEach(Proxies, (proxy) =>
                {
                    var result = proxyService.GetProxyInformationAsync(proxy.Host, proxy.Port, default).Result;


                    Application.Current.Dispatcher.Invoke(() => {

                        proxy.IsResponding = result.IsResponding;
                        proxy.Country = result.Country;
                        proxy.Delay = result.Delay;
                        proxy.WasVeryfiedAt = DateTime.Now;
                    }, DispatcherPriority.Background);

                });
            });
        }
    }
}
