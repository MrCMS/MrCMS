using System.ComponentModel;

namespace MrCMS.Settings
{
    public class SEOSettings : SiteSettingsBase
    {
        [DisplayName("Robots.txt file")]
        [TextArea]
        public string RobotsText { get; set; }

        [DisplayName("Tracking Scripts (head)")]
        [TextArea]
        public string TrackingScripts { get; set; }

        [DisplayName("Enable HTML Minification")]
        public bool EnableHtmlMinification { get; set; }

        public override bool RenderInSettings
        {
            get { return true; }
        }
    }
}