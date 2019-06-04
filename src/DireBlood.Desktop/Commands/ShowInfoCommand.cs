using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CheckProxy.Desktop.Commands.Abstractions;
using CheckProxy.Desktop.Properties;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CheckProxy.Desktop.Commands
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
