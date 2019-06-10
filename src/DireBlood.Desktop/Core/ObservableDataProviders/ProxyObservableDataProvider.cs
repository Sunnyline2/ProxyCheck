using System.Collections.Generic;
using System.Collections.ObjectModel;
using DireBlood.Core.Models;

namespace DireBlood.Core.ObservableDataProviders
{
    public class ProxyObservableDataProvider : IObservableDataProvider<Proxy>
    {
        private ObservableCollection<Proxy> proxies = new ObservableCollection<Proxy>();

        public void Add(Proxy item)
        {
            proxies.Add(item);
        }

        public void AddRange(IEnumerable<Proxy> items)
        {
            foreach (var proxy in items)
            {
                proxies.Add(proxy);
            }
        }

        public ObservableCollection<Proxy> Get()
        {
            return proxies;
        }

        public void Remove(Proxy item)
        {
            proxies.Remove(item);
        }

        public void Set(IEnumerable<Proxy> items)
        {
            proxies = new ObservableCollection<Proxy>(items);
        }
    }
}