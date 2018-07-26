using MrCMS.Settings;

namespace MrCMS.ACL
{
    public class ACLSettings : SiteSettingsBase
    {
        public override bool RenderInSettings
        {
            get { return false; }
        }

        public bool ACLEnabled { get; set; }
    }
}