using System.IO;
using System.Threading.Tasks;
using DireBlood.Core.Abstractions;
using DireBlood.Core.ObservableDataProviders;
using DireBlood.Core.Services;
using DireBlood.EventArgs;
using Microsoft.Win32;

namespace DireBlood.Commands
{
    public class SaveToFileCommand : ICommandFactory
    {
        private readonly object context;
        private readonly IJobService jobService;
        private readonly IObservableDataProvider<ProxyDetailsModel> proxyRepository;
        private readonly IStatusService statusService;

        public SaveToFileCommand(object context, IJobService jobService, IStatusService statusService,
            IObservableDataProvider<ProxyDetailsModel> proxyRepository)
        {
            this.context = context;
            this.jobService = jobService;
            this.statusService = statusService;
            this.proxyRepository = proxyRepository;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await SaveToFileAsync(), o => !jobService.IsRunning);
        }

        private async Task SaveToFileAsync()
        {
            if (!proxyRepository.GetAll().Any()) return;

            var saveDialog = new SaveFileDialog
            {
                Title = "Gdzie mam zapisać proxy?",
                DefaultExt = "plik tekstowy (.txt)|*.txt",
                OverwritePrompt = true
            };

            var dialogResult = saveDialog.ShowDialog();
            if (dialogResult == false) return;

            var job = new JobAsync<FileWritingEventArgs>((progress, args) => Task.Run(async () =>
                {
                    args.Count = proxyRepository.GetAll().Count;
                    progress.Report(args);

                    using (var fileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        for (var i = 0; i < args.Count; i++)
                        {
                            var proxy = proxyRepository.GetAll().ElementAt(i);
                            await streamWriter.WriteLineAsync($"{proxy.Host}:{proxy.Port}");
                        }
                    }
                }))
                .OnProgressChanged(args =>
                    statusService.SetStatus($"Trwa zapisywanie.. {args.Current}/{args.Count} {args.GetPercentage()}%"))
                .OnSuccess(args => statusService.SetStatus($"Pomyślnie zapisano {args.Count} adresów proxy."));

            await jobService.ExecuteAsync(job);
        }
    }
}