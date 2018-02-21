using System;
using CheckProxy.Core.Proxy;

namespace CheckProxy.Desktop.Models
{
    public class ProxyDetailsModel : BaseViewModel, IEquatable<ProxyDetailsModel>
    { 
        private string _host;

        public string Host
        {
            get => _host;
            set => Set(value, ref _host);
        }

        private ushort _port;

        public ushort Port
        {
            get => _port;
            set => Set(value, ref _port);
        }

        private string _country;

        public string Country
        {
            get => _country;
            set => Set(value, ref _country);
        }

        private long _delay;

        public long Delay
        {
            get => _delay;
            set => Set(value, ref _delay);
        }

        private ProxyType _proxyType;

        public ProxyType ProxyType
        {
            get => _proxyType;
            set => Set(value, ref _proxyType);
        }

        private bool _isResponding;

        public bool IsResponding
        {
            get => _isResponding;
            set => Set(value, ref _isResponding);
        }

        private bool _wasVeryfied;

        public bool WasVeryfied
        {
            get => _wasVeryfied;
            set => Set(value, ref _wasVeryfied);
        }

        private DateTime? _wasVeryfiedAt;

        public DateTime? WasVeryfiedAt
        {
            get => _wasVeryfiedAt;
            set => Set(value, ref _wasVeryfiedAt);
        }

        private ProxyStatus _status;

        public ProxyStatus Status
        {
            get => _status;
            set => Set(value, ref _status);
        }

        public void Update(ICheckProxyResult info, bool wasVeryfied)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (wasVeryfied)
            {
                WasVeryfied = true;
                WasVeryfiedAt = DateTime.Now;
            }


            Country = info.Country;
            Delay = info.Delay;
            ProxyType = info.ProxyType;
            IsResponding = info.IsResponding;
        }


        public bool Equals(ProxyDetailsModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_host, other._host) && _port == other._port;
        }


        public override int GetHashCode()
        {
            int hostCode = Host == null ? 0 : Host.GetHashCode();
            int portCode = Port.GetHashCode();
            return hostCode ^ portCode;
        }


        public enum ProxyStatus
        {
            Ready,
            InProcess,
        }
    }


}