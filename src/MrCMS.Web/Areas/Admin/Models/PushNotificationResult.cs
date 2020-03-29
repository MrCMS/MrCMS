using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class PushNotificationResult
    {
        [Required]
        public string Body { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}