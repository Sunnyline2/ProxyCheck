using CheckProxy.Desktop.Properties;
using MahApps.Metro.Controls.Dialogs;

namespace CheckProxy.Desktop.Factory
{
    public static class MetroDialogSettingsFactory
    {
        public static MetroDialogSettings Get()
        {
            return new MetroDialogSettings { AffirmativeButtonText = Resources.AffirmativeButtonText, NegativeButtonText = Resources.NegativeButtonText };
        }
    }
}
