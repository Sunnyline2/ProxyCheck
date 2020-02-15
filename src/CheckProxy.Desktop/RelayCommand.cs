using System;
using System.Windows.Input;

namespace CheckProxy.Desktop
{
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;

        #endregion

        #region Constructors

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        #endregion
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(o => execute())
        {
        }

        public RelayCommand(Action execute, Predicate<object> canExecute) : base(o => execute(), canExecute)
        {
        }
    }
}