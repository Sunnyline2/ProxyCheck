namespace CheckProxy.Core.Proxy
{
    public class CheckProxyResult : ICheckProxyResult
    {
        public string Host { get; }
        public ushort Port { get; set; }
        public bool IsResponding { get; set; }
        public long Delay { get; set; }
        public string Country { get; set; }
        public ProxyType ProxyType { get; set; }

        public CheckProxyResult(string host, ushort port)
        {
            Host = host;
            Port = port;
        }
    }
}