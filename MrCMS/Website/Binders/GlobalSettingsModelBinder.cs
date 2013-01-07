using System.Reflection;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Binders
{
    public class GlobalSettingsModelBinder : SettingModelBinder<GlobalSettingsBase>
    {
        protected override object[] Parameters(ISiteService sitesService, string siteId)
        {
            return new object[0];
        }

        protected override MethodInfo GetGetSettingsMethod()
        {
            return typeof(ConfigurationProvider).GetMethodExt("GetSettings");
        }
    }
}