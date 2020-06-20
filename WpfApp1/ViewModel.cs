using CheckProxy.Core.Proxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfApp1
{
    public class ViewModel
    {
        public ICommand LoadProxy { get; }
        public ObservableCollection<Proxy> Proxies { get; }

        public ViewModel()
        {
            Proxies = new ObservableCollection<Proxy>();
            LoadProxy = new Command(this);
        }
    }

    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly ViewModel viewModel;
        private readonly ProxyService proxyService = new ProxyService(new CheckProxy.Core.ProxyServiceConfiguration());

        public Command(ViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            viewModel.Proxies.Add(new Proxy("127.0.0.1", 70));


            var info =  await proxyService.GetProxyInformationsAsync(viewModel.Proxies[viewModel.Proxies.Count - 1].Host, viewModel.Proxies[viewModel.Proxies.Count - 1].Port, default);

            Task.Run(() =>
            {

            });
        }
    }
}
