using System.Threading.Tasks;
using DireBlood.Commands.Abstractions;
using DireBlood.Properties;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood.Commands
{
   public class ShowInfoCommand : ICommandFactory
    {
        private readonly MetroWindow _metroWindow;

        public ShowInfoCommand(MetroWindow metroWindow)
        {
            _metroWindow = metroWindow;
        }

        private async Task ShowInfoAsync()
        {
            await _metroWindow.ShowMessageAsync(Resources.Title, Resources.About);
        }

        public RelayCommand GetCommand()
        {
            return new RelayCommand(async () => await ShowInfoAsync());
        }
    }
}
