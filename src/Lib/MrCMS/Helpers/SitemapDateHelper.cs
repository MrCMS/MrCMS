using System;

namespace MrCMS.Helpers
{
    public static class SitemapDateHelper
    {
        public static string SitemapDateString(this DateTime dateTime)
        {
            return dateTime.ToString("O");
        }
    }
}