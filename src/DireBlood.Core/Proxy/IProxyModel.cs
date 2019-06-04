namespace DireBlood.Core.Proxy
{
    public interface IProxyModel
    {
        string Country { get; }
        long Delay { get; }
        string Host { get; }
        bool IsResponding { get; }
        ushort Port { get; }
        ProxyType ProxyType { get; }
    }
}