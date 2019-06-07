using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Core.Job;
using DireBlood.EventArgs;
using DireBlood.Models;
using DireBlood.Repository;
using DireBlood.Services;
using DireBlood.ViewModels;
using Microsoft.Win32;

namespace DireBlood.Commands
{
    public class SaveToFileCommand : ICommandFactory
    {
        private readonly object context;
        private readonly IJobManager jobManager;
        private readonly IStatusService statusService;
        private readonly IObservableRepository<ProxyDetailsModel> proxyRepository;

        public SaveToFileCommand(object context, IJobManager jobManager, IStatusService statusService, IObservableRepository<ProxyDetailsModel> proxyRepository)
        {
            this.context = context;
            this.jobManager = jobManager;
            this.statusService = statusService;
            this.proxyRepository = proxyRepository;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await SaveToFileAsync(), o => !jobManager.IsRunning);    
        }

        private async Task SaveToFileAsync()
        {
            if (!proxyRepository.GetAll().Any())
            {
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Title = "Gdzie mam zapisać proxy?",
                DefaultExt = "plik tekstowy (.txt)|*.txt",
                OverwritePrompt = true,
            };

            var dialogResult = saveDialog.ShowDialog();
            if (dialogResult == false)
            {
                return;

            }

            var job = new JobAsync<FileWritingEventArgs>((progress, args) => Task.Run(async () =>
                {
                    args.Count = proxyRepository.GetAll().Count;
                    progress.Report(args);

                    using (var fileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        for (int i = 0; i < args.Count; i++)
                        {
                            var proxy = proxyRepository.GetAll().ElementAt(i);
                            await streamWriter.WriteLineAsync($"{proxy.Host}:{proxy.Port}");
                        }
                    }
                }))
                .OnProgressChanged(args => statusService.SetStatus($"Trwa zapisywanie.. {args.Current}/{args.Count} {args.GetPergentage()}%"))
                .OnSuccess(args => statusService.SetStatus($"Pomyślnie zapisano {args.Count} adresów proxy."));

            await jobManager.ExecuteAsync(job);
        }
    }
}
