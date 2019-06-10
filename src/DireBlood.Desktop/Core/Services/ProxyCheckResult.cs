namespace DireBlood.Core.Services
{
    public class ProxyCheckResult : IProxyCheckResult
    {
        public bool IsResponding { get; set; }
        public string Country { get; set; }
        public int Delay { get; set; }
    }
}