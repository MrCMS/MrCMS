using System;
using System.ComponentModel;
using MrCMS.Entities.People;

namespace MrCMS.Entities.UserProfile
{
    public class NotificationSettings : UserProfileData
    {
        [DisplayName("Last Marked As Read")]
        public virtual DateTime? LastMarkedAsRead { get; set; }
        [DisplayName("Persistent Notifications Enabled")]
        public virtual bool PersistentNotificationsEnabled { get; set; }
        [DisplayName("Transient Notifications Enabled")]
        public virtual bool TransientNotificationsEnabled { get; set; }
    }
}