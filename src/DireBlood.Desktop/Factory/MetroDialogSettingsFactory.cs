using DireBlood.Properties;
using MahApps.Metro.Controls.Dialogs;

namespace DireBlood.Factory
{
    public static class MetroDialogSettingsFactory
    {
        public static MetroDialogSettings Get()
        {
            return new MetroDialogSettings { AffirmativeButtonText = Resources.AffirmativeButtonText, NegativeButtonText = Resources.NegativeButtonText };
        }
    }
}
