using MrCMS.Helpers;

namespace MrCMS.Services.Resources
{
    public class GetDefaultResourceValue : IGetDefaultResourceValue
    {
        public string GetValue(MissingLocalisationInfo info)
        {
            // If it already contains a space, it's likely already customised
            var value = info.Key;
            return GetDefaultValue(value);
        }

        public static string GetDefaultValue(string value)
        {
            return value.Contains(" ") ? value : value.BreakUpString();
        }
    }
}