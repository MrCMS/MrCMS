using System;
using System.ComponentModel;
using MrCMS.Entities.Notifications;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class NotificationSearchQuery
    {
        public NotificationSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        [DisplayName("Notification Type")]
        public NotificationType? NotificationType { get; set; }
    }
}