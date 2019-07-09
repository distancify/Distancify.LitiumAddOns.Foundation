using System;
using System.Collections.Generic;
using System.Linq;

namespace Distancify.LitiumAddOns.Foundation.Extensions
{
    public static class StringExtensions
    {
        public static List<string> GetIndividualLines(this string s)
        {
            return s.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n').ToList();
        }

        public static string TrimEnd(this string source, string value)
        {
            return source.EndsWith(value) ? source.Remove(source.LastIndexOf(value, StringComparison.Ordinal)) : source;
        }
    }
}