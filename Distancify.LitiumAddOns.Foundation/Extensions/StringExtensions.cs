using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Distancify.LitiumAddOns.Extensions
{
    public static class StringExtensions
    {
        public static List<string> GetIndividualLines(this string s)
        {
            return s.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').ToList();
        }

        public static string ToAbsoluteUrl(this string relativePath, string domainName, bool isSecureConnection)
        {
            return $"{(isSecureConnection ? "https" : "http")}://{domainName.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }

        public static string TrimEnd(this string source, string value)
        {
            return source.EndsWith(value) ? source.Remove(source.LastIndexOf(value, StringComparison.Ordinal)) : source;
        }

        public static int SeasonStringToNumber(this string season)
        {
            var regex = new Regex(@"^(?<half>SS|AW)(?<year>[0-9]{2})$");

            if (string.IsNullOrWhiteSpace(season)) throw new Exception("String is not season string.");

            var regexMatch = regex.Match(season);

            if (!regexMatch.Success) throw new Exception("String is not season string.");

            var seasonAsNumber = (int.Parse(regexMatch.Groups["year"].Value) * 10) + (regexMatch.Groups["half"].Equals("SS") ? 1 : 2);

            return seasonAsNumber;
        }
    }
}