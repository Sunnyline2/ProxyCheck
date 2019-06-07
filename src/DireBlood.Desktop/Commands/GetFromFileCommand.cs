using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Core;
using DireBlood.Core.Job;
using DireBlood.EventArgs;
using DireBlood.Models;
using DireBlood.Repository;
using DireBlood.Services;
using DireBlood.Utilities;
using DireBlood.ViewModels;
using Microsoft.Win32;

namespace DireBlood.Commands
{
    public class GetFromFileCommand : ICommandFactory
    {
        private readonly object context;
        private readonly IJobManager jobManager;
        private readonly IObservableRepository<ProxyDetailsModel> proxyDetailsModels;
        private readonly IStatusService statusService;

        public GetFromFileCommand(object context, IJobManager jobManager, IObservableRepository<ProxyDetailsModel> proxyDetailsModels, IStatusService statusService)
        {
            this.context = context;
            this.jobManager = jobManager;
            this.proxyDetailsModels = proxyDetailsModels;
            this.statusService = statusService;
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

            if (fileDialog.ShowDialog() == false)
                return;

            var job = new JobAsync<FileReadingEventArgs>((progress, args) => Task.Run(async () =>
            {
                using (var fileStream = new FileStream(fileDialog.FileName, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                {
                    args.Count = streamReader.CountLines();
                    progress.Report(args);

                    var collection = new List<ProxyDetailsModel>();

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

                    proxyDetailsModels.AddRange(collection);
                }
            }))
                .OnProgressChanged(args => statusService.SetStatus($"Wczytywanie.. {args.Current}/{args.Count} {args.GetPergentage()}%"))
                .OnSuccess(args => statusService.SetStatus($"Pomyślnie wczytano {args.Count} adresów proxy."))
                .OnException(exception => statusService.SetStatus($"Wystąpił problem podczas wczytywania listy proxy. {exception.Message}"));

            await jobManager.ExecuteAsync(job);
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await GetFromFileAsync(), o => !jobManager.IsRunning);
        }
    }
}
