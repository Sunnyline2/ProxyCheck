using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CheckProxy.Desktop;
using CheckProxy.Desktop.Models;

namespace CheckProxy.Core.Proxy
{
    public class Proxy : BaseNotifyPropertyChanged
    {
        public string Host { get; }

        public ushort Port { get; }

        private bool? isResponding;

        public bool? IsResponding
        {
            get => isResponding;
            set => Set(ref isResponding, value);
        }

        private ulong? delay;

        public ulong? Delay
        {
            get => delay;
            set => Set(ref delay, value);
        }

        private string country;

        public string Country
        {
            get => country;
            set => Set(ref country, value);
        }

        private ProxyType type;

        public ProxyType Type
        {
            get => type;
            set => Set(ref type, value);
        }

        private ProxyStatus status;

        public ProxyStatus Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private DateTimeOffset? wasVeryfiedAt;

        public DateTimeOffset? WasVeryfiedAt
        {
            get => wasVeryfiedAt;
            set => Set(ref wasVeryfiedAt, value);
        }

        private DateTimeOffset createdAt;

        public DateTimeOffset CreatedAt
        {
            get => createdAt;
            set => Set(ref createdAt, value);
        }

        public Proxy(string host, ushort port)
        {
            if (string.IsNullOrEmpty(host)) throw new ArgumentException(nameof(host));
            if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));
            if (!IsValidHost(host)) throw new ArgumentException(nameof(host));

            Host = host;
            Port = port;

            CreatedAt = DateTimeOffset.Now;
        }

        private bool IsValidHost(string host)
        {
            const int IP_ADDRESS_MIN_DOTS = 3;
            return host.Count(x => x.Equals('.')).Equals(IP_ADDRESS_MIN_DOTS) && IPAddress.TryParse(host, out var _);
        }

        public override string ToString()
        {
            return string.Concat(Host, ":", Port);
        }

        public override bool Equals(object obj)
        {
            return obj is Proxy proxy && string.Equals(Host, proxy.Host) && Port == proxy.Port;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Proxy left, Proxy right)
        {
            return EqualityComparer<Proxy>.Default.Equals(left, right);
        }

        public static bool operator !=(Proxy left, Proxy right)
        {
            return !(left == right);
        }
    }
}