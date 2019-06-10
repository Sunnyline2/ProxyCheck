using System;
using System.Windows.Input;

namespace DireBlood.Core.Abstractions
{
    public class BasicRelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public BasicRelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}