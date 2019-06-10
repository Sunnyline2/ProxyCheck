using System;
using GalaSoft.MvvmLight;

namespace DireBlood.Core.Models
{
    public class Proxy : ViewModelBase, IEquatable<Proxy>
    {
        private ProxyOperation currentOperation;

        private string host;

        private ushort port;

        public Proxy(string host, ushort port)
        {
            Host = host;
            Port = port;
        }

        public string Host
        {
            get => host;
            set => Set(ref host, value);
        }

        public ushort Port
        {
            get => port;
            set => Set(ref port, value);
        }

        public ProxyOperation CurrentOperation
        {
            get => currentOperation;
            set => Set(ref currentOperation, value);
        }

        public ProxyDetails Details { get; } = new ProxyDetails();
        public ProxyStatus Status { get; } = new ProxyStatus();

        public bool Equals(Proxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(host, other.host, StringComparison.InvariantCultureIgnoreCase) && port == other.port;
        }
    }
}