using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using CheckProxy.Desktop.Commands.Abstractions;
using CheckProxy.Desktop.Factory;
using CheckProxy.Desktop.Properties;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CheckProxy.Desktop.Commands
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
