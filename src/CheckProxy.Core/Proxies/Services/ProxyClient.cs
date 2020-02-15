using System;
using System.Net;
using System.Net.Http;

namespace CheckProxy.Core.Proxy
{
    public class ProxyClient : HttpClient
    {
        public ProxyClient(string host, ushort port, TimeSpan timeout) : base(GetHandlerForProxy(host, port))
        {
            Timeout = timeout;
        }

        private static HttpClientHandler GetHandlerForProxy(string host, ushort port)
        {
            return new HttpClientHandler
            {
                Proxy = new WebProxy(string.Concat(host, ":", port)),
                UseProxy = true,
                AllowAutoRedirect = false
            };
        }
    }
}