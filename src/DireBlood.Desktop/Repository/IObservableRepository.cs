using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DireBlood.Repository
{
    public interface IObservableRepository<T>
    {
        ObservableCollection<T> GetAll();

        void Add(T item);
        void AddRange(IEnumerable<T> items);
        void Remove(T item);

    }
}