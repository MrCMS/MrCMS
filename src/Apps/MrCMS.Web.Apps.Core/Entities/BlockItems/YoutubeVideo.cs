using MrCMS.Entities.Documents.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace MrCMS.Web.Apps.Core.Entities.BlockItems
{
    [Display(Name = "Youtube")]
    public class YoutubeVideo : BlockItem
    {
        public YoutubeVideo()
        {
            EnablePrivacyMode = true;
        }
        public string VideoId { get; set; }
        public bool Autoplay { get; set; }
        public bool EnablePrivacyMode { get; set; }
        public bool ShowPlayerControl { get; set; }
        public string StartAt { get; set; }

        public string GetHtml()
        {
            var url = new StringBuilder(EnablePrivacyMode ? "https://www.youtube-nocookie.com/embed/" : "https://www.youtube.com/embed/");

            url.Append(VideoId).Append("?");
            url.Append("autoplay=").Append(Autoplay ? "1" : "0");
            url.Append("&controls=").Append(ShowPlayerControl ? "1" : "0");

            if (TimeSpan.TryParseExact(StartAt, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out var startTieme))
            {
                url.Append("&start=").Append(startTieme.TotalSeconds);
            }

            return $"<iframe src='{url}' frameborder='0' allow='accelerometer; autoplay;  clipboard-write; encrypted-media; gyroscope; picture-in-picture'></iframe>";
        }
    }
}
