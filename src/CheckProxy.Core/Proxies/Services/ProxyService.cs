using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CheckProxy.Core.Proxy
{
    public class ProxyService : IProxyService
    {
        public const string PROXY_JUDGE_ENDPOINT = "http://www.cooleasy.com/azenv.php";

        public static readonly Lazy<Regex> ProxyJudgeRegex = new Lazy<Regex>(() => new Regex("([A-Z].+) = ([A-z-0-9].+)", RegexOptions.Compiled));

        private readonly ProxyServiceConfiguration proxyServiceConfiguration;

        public ProxyService(ProxyServiceConfiguration proxyServiceConfiguration)
        {
            this.proxyServiceConfiguration = proxyServiceConfiguration ?? throw new ArgumentNullException(nameof(proxyServiceConfiguration));
        }

        public async Task<IProxyInfo> GetProxyInformationAsync(string host, ushort port, CancellationToken cancellationToken)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));

            using (var proxyClient = new ProxyClient(host, port, proxyServiceConfiguration.Timeout))
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    using (var responseMessage = await proxyClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, PROXY_JUDGE_ENDPOINT), cancellationToken))
                    {
                        responseMessage.EnsureSuccessStatusCode();

                        using (var httpContent = responseMessage.Content)
                        {
                            var content = await httpContent.ReadAsStringAsync();
                            var matches = GetMatches(content);

                            return new ProxyInfo(GetValue(matches, "HTTP_CF_IPCOUNTRY"), true, (ulong)stopwatch.ElapsedMilliseconds);
                        }
                    }
                }
                catch (Exception)
                {
                    return new ProxyInfo(string.Empty, false, null);
                }
            }
        }

        private static MatchCollection GetMatches(string content)
        {
            return ProxyJudgeRegex.Value.Matches(content);
        }

        private static string GetValue(MatchCollection matches, string name)
        {
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                if (match.Groups[1].Value == name)
                {
                    return match.Groups[2].Value;
                }
            }
            return string.Empty;
        }
    }

    public class ProxyInfo : IProxyInfo
    {
        public string Country { get; }

        public bool IsResponding { get; }

        public ulong? Delay { get; }

        public ProxyInfo(string country, bool isResponding, ulong? delay)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));
            IsResponding = isResponding;
            Delay = delay;
        }
    }
}