namespace CheckProxy.Core.Proxy
{
    public interface IProxyInfo
    {
        string Country { get; }
        bool IsResponding { get; }
        ulong? Delay { get; }
    }
}