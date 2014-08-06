using System;
using System.Drawing;
using System.Text;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class MediaSettingExtensions
    {
        public static CachingInfo GetCachingInfo(this MediaSettings mediasettings, string imageUrl, Size targetSize = default(Size), string alt = null, string title = null, object attributes = null)
        {
            var cacheKey = GetCacheKey(imageUrl, targetSize, alt, title,attributes);
            return new CachingInfo(mediasettings.Cache, cacheKey, TimeSpan.FromSeconds(mediasettings.CacheLength),mediasettings.CacheExpiryType);
        }

        private static string GetCacheKey(string imageUrl, Size targetSize, string alt, string title, object attributes)
        {
            var stringBuilder = new StringBuilder("RenderImage." + imageUrl);
            if (targetSize != default(Size))
                stringBuilder.AppendFormat(";size:{0},{1}", targetSize.Width, targetSize.Height);
            if (!string.IsNullOrWhiteSpace(alt))
                stringBuilder.AppendFormat(";alt:{0}", alt);
            if (!string.IsNullOrWhiteSpace(title))
                stringBuilder.AppendFormat(";title:{0}", title);
            if (!string.IsNullOrWhiteSpace(title))
                stringBuilder.AppendFormat(";title:{0}", title);
            if (attributes != null)
            {
                var routeValueDictionary = MrCMSHtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary)
                {
                    stringBuilder.AppendFormat(";{0}:{1}", kvp.Key, kvp.Value);
                }
            }

            return stringBuilder.ToString();
        }
    }
}