using System;
using DireBlood.Core.Proxy;
using GalaSoft.MvvmLight;

namespace CheckProxy.Desktop.Models
{
    public class ProxyDetailsModel : ViewModelBase, IEquatable<ProxyDetailsModel>
    { 
        private string _host;

        public string Host
        {
            get => _host;
            set => Set( ref _host, value );
        }

        private ushort _port;

        public ushort Port
        {
            get => _port;
            set => Set(ref _port, value);
        }

        private string _country;

        public string Country
        {
            get => _country;
            set => Set( ref _country, value);
        }

        private long _delay;

        public long Delay
        {
            get => _delay;
            set => Set( ref _delay, value);
        }

        private ProxyType _proxyType;

        public ProxyType ProxyType
        {
            get => _proxyType;
            set => Set( ref _proxyType, value);
        }

        private bool _isResponding;

        public bool IsResponding
        {
            get => _isResponding;
            set => Set(ref _isResponding, value);
        }

        private bool _wasVeryfied;

        public bool WasVeryfied
        {
            get => _wasVeryfied;
            set => Set(ref _wasVeryfied, value);
        }

        private DateTime? _wasVeryfiedAt;

        public DateTime? WasVeryfiedAt
        {
            get => _wasVeryfiedAt;
            set => Set(ref _wasVeryfiedAt, value);
        }

        private ProxyStatus _status;

        public ProxyStatus Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public void Update(IProxyModel info, bool wasVeryfied)
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


        public enum ProxyStatus
        {
            Ready,
            InProcess,
        }
    }
}