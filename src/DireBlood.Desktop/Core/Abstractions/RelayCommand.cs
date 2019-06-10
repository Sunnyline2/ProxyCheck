using System;

namespace DireBlood.Core.Abstractions
{
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