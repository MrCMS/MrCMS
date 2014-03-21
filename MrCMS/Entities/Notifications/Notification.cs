using System;
using MrCMS.Entities.People;

namespace MrCMS.Entities.Notifications
{
    public class Notification : SiteEntity
    {
        public virtual string Message { get; set; }
        public virtual User User { get; set; }
        public virtual NotificationType NotificationType { get; set; }

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