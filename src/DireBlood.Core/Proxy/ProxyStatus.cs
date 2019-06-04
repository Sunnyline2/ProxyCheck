namespace DireBlood.Core.Proxy
{
    public class ProxyStatus : IProxyStatus
    {
        public string Host { get; }
        public ushort Port { get; set; }
        public bool IsResponding { get; set; }
        public long Delay { get; set; }
        public string Country { get; set; }
        public ProxyType ProxyType { get; set; }

        public ProxyStatus(string host, ushort port)
        {
            Host = host;
            Port = port;
        }
    }
}