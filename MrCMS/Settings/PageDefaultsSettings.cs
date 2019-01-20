using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Settings
{
    public class PageDefaultsSettings : SiteSettingsBase
    {
        public PageDefaultsSettings()
        {
            UrlGenerators = new Dictionary<string, string>();
            Layouts = new Dictionary<string, int?>();
            DisableCaches = new HashSet<string>();
        }

        public Dictionary<string, string> UrlGenerators { get; set; }
        public Dictionary<string, int?> Layouts { get; set; }
        public HashSet<string> DisableCaches { get; set; }

        public Type GetGeneratorType(string pageType)
        {
            Type type = null;
            if (UrlGenerators.ContainsKey(pageType))
            {
                string urlGeneratorType = UrlGenerators[pageType];

                type = TypeHelper.GetTypeByName(urlGeneratorType);
            }
            return type ?? typeof(DefaultWebpageUrlGenerator);
        }

        public Type GetGeneratorType(Type pageType)
        {
            return pageType == null
                ? typeof(DefaultWebpageUrlGenerator)
                : GetGeneratorType(pageType.FullName);
        }

        public int? GetLayoutId(string pageType)
        {
            return Layouts.ContainsKey(pageType)
                ? Layouts[pageType]
                : null;
        }
        public int? GetLayoutId(Type type)
        {
            return GetLayoutId(type.FullName);
        }

        public bool CacheDisabled(string pageType)
        {
            return DisableCaches.Contains(pageType);
        }

        public bool CacheDisabled(Type type)
        {
            return CacheDisabled(type.FullName);
        }
    }
}