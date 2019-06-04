using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Core;
using DireBlood.Core.Job;
using DireBlood.EventArgs;
using DireBlood.Models;
using DireBlood.Utilities;
using DireBlood.ViewModels;
using Microsoft.Win32;

namespace DireBlood.Commands
{
    public class GetFromFileCommand : ICommandFactory
    {
        private readonly MainViewModel mainViewModel;

        public GetFromFileCommand(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
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

            if (fileDialog.ShowDialog(mainViewModel.Window) == false)
                return;

            var job = new JobAsync<FileReadingEventArgs>((progress, args) => Task.Run(async () =>
            {
                using (var fileStream = new FileStream(fileDialog.FileName, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                {
                    args.Count = streamReader.CountLines();
                    progress.Report(args);

                    var collection = new ObservableCollection<ProxyDetailsModel>(mainViewModel.ProxyViewModels);

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

                    mainViewModel.ProxyViewModels = collection;
                }
            }))
                .OnProgressChanged(args => mainViewModel.SetStatus($"Wczytywanie.. {args.Current}/{args.Count} {args.GetPergentage()}%"))
                .OnSuccess(args => mainViewModel.SetStatus($"Pomyślnie wczytano {args.Count} adresów proxy."))
                .OnException(exception => mainViewModel.SetStatus($"Wystąpił problem podczas wczytywania listy proxy. {exception.Message}"));

            await mainViewModel.JobManager.ExecuteAsync(job);
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await GetFromFileAsync(), o => !mainViewModel.JobManager.IsRunning);
        }
    }
}
