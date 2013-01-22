using System;
using System.Reflection;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Binders
{
    public class SiteSettingsModelBinder : SettingModelBinder<SiteSettingsBase>
    {
        protected override MethodInfo GetGetSettingsMethod()
        {
            return typeof(ConfigurationProvider).GetMethodExt("GetSiteSettings");
        }
    }
}