using GalaSoft.MvvmLight;

namespace DireBlood.Core.Models
{
    public class ProxyDetails : ViewModelBase
    {
        private string country;

        private long delay;

        private ProxyType proxyType;

        public string Country
        {
            get => country;
            set => Set(ref country, value);
        }

        public long Delay
        {
            get => delay;
            set => Set(ref delay, value);
        }

        public ProxyType ProxyType
        {
            get => proxyType;
            set => Set(ref proxyType, value);
        }
    }
}