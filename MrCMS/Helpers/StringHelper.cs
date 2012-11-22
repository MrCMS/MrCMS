using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace MrCMS.Helpers
{
    public static class StringHelper
    {
        public static string BreakUpString(this string value)
        {
            return Regex.Replace(value,
                                 "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))",
                                 " $1",
                                 RegexOptions.Compiled).Trim();
        }

        public static List<int> GetIntList(this string value)
        {
            return string.IsNullOrWhiteSpace(value)
                       ? new List<int>()
                       : value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s.Trim()))
                             .ToList();
        }

        public static string ToString(this IEnumerable<int> value)
        {
            if (value == null || !value.Any())
                return string.Empty;

            return string.Join(",", value);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
