using System;
using System.Text.RegularExpressions;

namespace DireBlood.Core.Utilities
{
    public static class RegexInstances
    {
        public static readonly Lazy<Regex> ProxyRegex = new Lazy<Regex>(() =>
            new Regex(@"(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}):(\d{1,6})", RegexOptions.Compiled));

        public static readonly Lazy<Regex> ProxyJudgeRegex =
            new Lazy<Regex>(() => new Regex("([A-Z].+) = ([A-z-0-9].+)", RegexOptions.Compiled));
    }
}