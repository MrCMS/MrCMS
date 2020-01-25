using System;
using MrCMS.Entities.People;

namespace MrCMS.Entities.Notifications
{
    public class Notification : SiteEntity
    {
        public string Message { get; set; }
        public User User { get; set; }
        public int? UserId { get; set; }
        public NotificationType NotificationType { get; set; }

        public virtual string UserName
        {
            get { return User != null ? User.Name : "-"; }
        }
    }

    public enum NotificationType
    {
        AdminOnly,
        UserOnly,
        All
    }
}