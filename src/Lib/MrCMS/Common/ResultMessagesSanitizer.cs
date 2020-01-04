using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Common
{
    public static class ResultMessagesSanitizer
    {
        public static ICollection<string> Sanitize(string[] messages)
        {
            return (messages ?? new string[0]).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }
    }
}