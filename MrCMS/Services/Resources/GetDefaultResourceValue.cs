using MrCMS.Helpers;

namespace MrCMS.Services.Resources
{
    public class GetDefaultResourceValue : IGetDefaultResourceValue
    {
        public string GetValue(MissingLocalisationInfo info)
        {
            // If it already contains a space, it's likely already customised
            if (info.Key.Contains(" "))
                return info.Key;

            return info.Key.BreakUpString();
        }
    }
}