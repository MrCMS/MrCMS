using System.Reflection;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Binders
{
    public class GlobalSettingsModelBinder : SettingModelBinder<GlobalSettingsBase>
    {
        protected override MethodInfo GetGetSettingsMethod()
        {
            return typeof(ConfigurationProvider).GetMethodExt("GetGlobalSettings");
        }
    }
}