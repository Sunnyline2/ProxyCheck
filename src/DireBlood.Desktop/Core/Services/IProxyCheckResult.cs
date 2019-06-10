namespace DireBlood.Core.Services
{
    public interface IProxyCheckResult
    {
        bool IsResponding { get; }
        string Country { get; }
        int Delay { get; }
    }
}