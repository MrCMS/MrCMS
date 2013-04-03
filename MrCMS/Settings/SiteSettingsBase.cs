using MrCMS.Entities.Multisite;

namespace MrCMS.Settings
{
    public abstract class SiteSettingsBase : SettingsBase
    {
        public Site Site { get; set; }
    }
}