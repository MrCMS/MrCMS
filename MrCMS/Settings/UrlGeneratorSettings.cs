using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Settings
{
    public class UrlGeneratorSettings : SiteSettingsBase
    {
        public UrlGeneratorSettings()
        {
            Defaults = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Defaults { get; set; }

        public Type GetGeneratorType(string pageType)
        {
            Type type = null;
            if (Defaults.ContainsKey(pageType))
            {
                string urlGeneratorType = Defaults[pageType];

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
    }
}