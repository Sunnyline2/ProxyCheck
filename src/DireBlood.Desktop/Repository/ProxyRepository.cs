using System.Collections.Generic;
using System.Collections.ObjectModel;
using DireBlood.Models;

namespace DireBlood.Repository
{
    public class ProxyRepository : IObservableRepository<ProxyDetailsModel>
    {
        private readonly ObservableCollection<ProxyDetailsModel> proxyDetailsModels = new ObservableCollection<ProxyDetailsModel>();

        public ObservableCollection<ProxyDetailsModel> GetAll()
        {
            return proxyDetailsModels;
        }

        public void Add(ProxyDetailsModel proxyDetailsModel)
        {
            proxyDetailsModels.Add(proxyDetailsModel);
        }

        public void AddRange(IEnumerable<ProxyDetailsModel> items)
        {
            foreach (var proxyDetailsModel in items)
            {
                Add(proxyDetailsModel);
            }
        }

        public void Remove(ProxyDetailsModel proxyDetailsModel)
        {
            proxyDetailsModels.Remove(proxyDetailsModel);
        }
    }
}