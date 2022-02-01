using System.ComponentModel;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Content
{
    public class UpdateYoutubeVideoAdminModel
    {
        [DisplayName("Youtube Video Id")]
        public string VideoId { get; set; }

        [DisplayName("Autoplay")]
        public bool Autoplay { get; set; }

        [DisplayName("Enable Privacy Mode")]
        public bool EnablePrivacyMode { get; set; }

        [DisplayName("Show Player Controls")]
        public bool ShowPlayerControl { get; set; }

        [DisplayName("Start At (hh:mm:ss)")]
        public string StartAt { get; set; }
    }
}
