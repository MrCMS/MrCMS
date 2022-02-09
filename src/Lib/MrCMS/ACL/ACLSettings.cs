using MrCMS.Settings;

namespace MrCMS.ACL
{
    public class ACLSettings : SiteSettingsBase
    {
        public override bool RenderInSettings => false;

        public bool ACLEnabled { get; set; }
    }
}