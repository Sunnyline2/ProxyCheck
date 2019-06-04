namespace DireBlood.Core.Proxy
{
    public interface IProxyStatus
    {
        string Country { get; }
        long Delay { get; }
        string Host { get; }
        bool IsResponding { get; }
        ushort Port { get; }
        ProxyType ProxyType { get; }
    }
}