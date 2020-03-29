using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class PushNotificationModel
    {
        [Required]
        public string Body { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Badge { get; set; }
    }
}