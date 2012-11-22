using System.Text.RegularExpressions;

namespace MrCMS.Helpers
{
    public static class SeoHelper
    {
        //returns a neat url, lower case, allows: -_/0-9a-z
        public static string TidyUrl(string url)
        {
            //Todo: Limit length of url to whatever google says
            if (string.IsNullOrWhiteSpace(url))
                return "";
            url = url.ToLower().Replace("&", "and").Replace(".", "").Trim();
            url = Regex.Replace(url.ToLower(), @"[^\w-//]+", "-");
            url = url.Trim('/');
            return url;
        }
    }
}
