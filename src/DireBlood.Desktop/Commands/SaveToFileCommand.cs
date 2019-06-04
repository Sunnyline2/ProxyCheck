using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Core.Job;
using DireBlood.EventArgs;
using DireBlood.ViewModels;
using Microsoft.Win32;

namespace DireBlood.Commands
{
    public class SaveToFileCommand : ICommandFactory
    {
        private readonly MainViewModel mainViewModel;

        public SaveToFileCommand(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await SaveToFileAsync(), o => !mainViewModel.JobManager.IsRunning);    
        }

        private async Task SaveToFileAsync()
        {
            if (mainViewModel.ProxyViewModels.Any() == false)
                return;

            var saveDialog = new SaveFileDialog
            {
                Title = "Gdzie mam zapisać proxy?",
                DefaultExt = "plik tekstowy (.txt)|*.txt",
                OverwritePrompt = true,
            };

            var dialogResult = saveDialog.ShowDialog(mainViewModel.Window);
            if (dialogResult == false)
                return;

            var job = new JobAsync<FileWritingEventArgs>((progress, args) => Task.Run(async () =>
                {
                    args.Count = mainViewModel.ProxyViewModels.Count;
                    progress.Report(args);

                    using (var fileStream = new FileStream(saveDialog.FileName, FileMode.Create))
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        for (int i = 0; i < mainViewModel.ProxyViewModels.Count; i++)
                        {
                            var proxy = mainViewModel.ProxyViewModels.ElementAt(i);
                            await streamWriter.WriteLineAsync($"{proxy.Host}:{proxy.Port}");
                        }
                    }
                }))
                .OnProgressChanged(args => mainViewModel.SetStatus($"Trwa zapisywanie.. {args.Current}/{args.Count} {args.GetPergentage()}%"))
                .OnSuccess(args => mainViewModel.SetStatus($"Pomyślnie zapisano {args.Count} adresów proxy."));

            await mainViewModel.JobManager.ExecuteAsync(job);
        }
    }
}
