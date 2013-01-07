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
        protected override object[] Parameters(ISiteService sitesService, string siteId)
        {
            return new object[]
                       {
                           sitesService.GetSite(Convert.ToInt32(siteId))
                       };
        }

        protected override MethodInfo GetGetSettingsMethod()
        {
            return typeof(ConfigurationProvider).GetMethodExt("GetSettings", typeof(Site));
        }
    }
}