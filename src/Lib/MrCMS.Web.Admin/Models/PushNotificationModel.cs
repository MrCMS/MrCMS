using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class PushNotificationModel
    {
        [Required]
        public string Body { get; set; }

        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Badge { get; set; }
        public string Image { get; set; }
    }
}