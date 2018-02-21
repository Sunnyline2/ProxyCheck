namespace CheckProxy.Core.Proxy
{
    public interface ICheckProxyResult
    {
        string Country { get; set; }
        long Delay { get; set; }
        string Host { get; }
        bool IsResponding { get; set; }
        ushort Port { get; set; }
        ProxyType ProxyType { get; set; }
    }
}