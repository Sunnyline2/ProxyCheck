using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DireBlood.Core.ObservableDataProviders
{
    public interface IObservableDataProvider<T>
    {
        ObservableCollection<T> Get();
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        void Remove(T item);
        void Set(IEnumerable<T> items);
    }
}