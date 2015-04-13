using MrCMS.Services.Resources;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class StringResourceHelper
    {
        public static string AsResource(this string value)
        {
            var provider = MrCMSApplication.Get<IStringResourceProvider>();

            return provider.GetValue(value);
        }
    }
}