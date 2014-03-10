using MrCMS.Services.Resources;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class MrCMSResources
    {
        public static string Get(string key, string defaultValue = null)
        {
            return MrCMSApplication.Get<IStringResourceProvider>().GetValue(key, defaultValue);
        }
    }
}