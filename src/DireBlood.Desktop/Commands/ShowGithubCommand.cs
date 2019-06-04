using System.Diagnostics;
using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Factory;
using DireBlood.Properties;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood.Commands
{
    public class ShowGithubCommand : ICommandFactory
    {
        private readonly MetroWindow metroWindow;

        public ShowGithubCommand(MetroWindow metroWindow)
        {
            this.metroWindow = metroWindow;
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await ShowGithubInfoAsync());
        }

        private async Task ShowGithubInfoAsync()
        {
            var dialogResult = await metroWindow.ShowMessageAsync(Resources.Title, Resources.Redir, MessageDialogStyle.AffirmativeAndNegative, MetroDialogSettingsFactory.Get());
            if (dialogResult == MessageDialogResult.Affirmative)
            {
                Process.Start("http://github.com/nickofc/");
            }
        }
    }
}
