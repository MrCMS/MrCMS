using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;

namespace MrCMS.Web.Admin.Models
{
    public class NotificationModel
    {
        [Required]
        public string Message { get; set; }
        [DisplayName("Publish Type")]
        public PublishType PublishType { get; set; }
        [DisplayName("Notification Type")]
        public NotificationType NotificationType { get; set; }
    }
}