using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckProxy.Desktop
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(T value, ref T instance, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, instance))
            {
                return false;
            }

            instance = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}