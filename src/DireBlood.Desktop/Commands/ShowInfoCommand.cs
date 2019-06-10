using System.Threading.Tasks;
using System.Windows.Input;
using DireBlood.Core.Abstractions;
using DireBlood.Properties;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood.Commands
{
    public class ShowInfoCommand : ICommandFactory
    {
        private readonly object context;
        private readonly IDialogCoordinator dialogCoordinator;

        public ShowInfoCommand(object context, IDialogCoordinator dialogCoordinator)
        {
            this.context = context;
            this.dialogCoordinator = dialogCoordinator;
        }

        public ICommand Get()
        {
            return new RelayCommand(async () => await ShowInfoAsync());
        }

        private async Task ShowInfoAsync()
        {
            await dialogCoordinator.ShowMessageAsync(context, Resources.Title, Resources.About);
        }
    }
}