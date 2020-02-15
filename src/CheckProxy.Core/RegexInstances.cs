using System;
using System.Text.RegularExpressions;

namespace CheckProxy.Core
{
    public static class RegexInstances
    {
        public static readonly Lazy<Regex> ProxyRegex = new Lazy<Regex>(() =>
            new Regex(@"(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,6})", RegexOptions.Compiled));
    }
}