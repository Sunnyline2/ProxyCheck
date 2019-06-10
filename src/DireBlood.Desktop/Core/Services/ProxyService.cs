using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DireBlood.Core.Utilities;

namespace DireBlood.Core.Services
{
    public class ProxyService : IProxyService
    {
        private const string CountryKey = "HTTP_CF_IPCOUNTRY";

        public async Task<IProxyCheckResult> CheckAsync(string host, ushort port, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));

            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.Proxy = new WebProxy(string.Concat(host, ":", port));
                    handler.UseProxy = true;
                    handler.AllowAutoRedirect = false;

                    using (var httpClient = new HttpClient(handler) {Timeout = timeout})
                    {
                        var stopwatch = Stopwatch.StartNew();
                        using (var responseMessage = await httpClient.SendAsync(
                            new HttpRequestMessage(HttpMethod.Get, "http://www.cooleasy.com/azenv.php"),
                            cancellationToken))
                        {
                            responseMessage.EnsureSuccessStatusCode();

                            using (var httpContent = responseMessage.Content)
                            {
                                var content = await httpContent.ReadAsStringAsync();
                                var matches = GetMatches(content);

                                return new ProxyCheckResult
                                {
                                    Country = GetValue(matches, CountryKey),
                                    IsResponding = true,
                                    Delay = (int)stopwatch.ElapsedMilliseconds
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException canceledException)
                {
                    if (canceledException.CancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                }
                return new ProxyCheckResult {IsResponding = false};
            }
        }

        private static MatchCollection GetMatches(string content)
        {
            return RegexInstances.ProxyJudgeRegex.Value.Matches(content);
        }

        private static string GetValue(MatchCollection matches, string name)
        {
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                if (match.Groups[1].Value == name) return match.Groups[2].Value;
            }

            return string.Empty;
        }
    }
}