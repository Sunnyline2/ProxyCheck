using CommonServiceLocator;
using DireBlood.Core.Services;
using DireBlood.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IProxyService, ProxyService>();
            SimpleIoc.Default.Register<IJobService, JobService>();
            SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();
            SimpleIoc.Default.Register<IStatusService, StatusService>();
            SimpleIoc.Default.Register<IProxyCheckRunner, ProxyCheckRunner>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}