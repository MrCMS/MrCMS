using MrCMS.Settings;

namespace MrCMS.Data
{
    public class AuditSettings : SiteSettingsBase
    {
        public AuditLevel AuditLevel { get; set; }
    }
}