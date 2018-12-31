using System;
using System.Drawing;
using System.Text;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class MediaSettingExtensions
    {
        public const string RenderTagPrefix = "RenderImage.Tag.";
        public const string RenderUrlPrefix = "RenderImage.Url.";

        public static CachingInfo GetImageTagCachingInfo(this MediaSettings mediasettings, string imageUrl, Size targetSize = default(Size), string alt = null, string title = null, object attributes = null)
        {
            var cacheKey = GetCacheKey(RenderTagPrefix, imageUrl, targetSize, alt, title, attributes);
            return new CachingInfo(mediasettings.Cache, cacheKey, TimeSpan.FromSeconds(mediasettings.CacheLength), mediasettings.CacheExpiryType);
        }

        public static CachingInfo GetImageUrlCachingInfo(this MediaSettings mediasettings, string imageUrl, Size targetSize = default(Size))
        {
            var cacheKey = GetCacheKey(RenderUrlPrefix, imageUrl, targetSize);
            return new CachingInfo(mediasettings.Cache, cacheKey, TimeSpan.FromSeconds(mediasettings.CacheLength), mediasettings.CacheExpiryType);
        }

        private static string GetCacheKey(string prefix, string imageUrl, Size targetSize, string alt = null, string title = null, object attributes = null)
        {
            var stringBuilder = new StringBuilder(prefix + imageUrl);
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
                var routeValueDictionary = MrCMSHtmlHelperExtensions.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var kvp in routeValueDictionary)
                {
                    stringBuilder.AppendFormat(";{0}:{1}", kvp.Key, kvp.Value);
                }
            }

            return stringBuilder.ToString();
        }
    }
}