using MrCMS.Entities.People;
using MrCMS.Entities.UserProfile;
using MrCMS.Services.Events.Args;

namespace MrCMS.Services.Events
{
    public class InitializeNotificationSettings : IOnUserAdded, IInitializeNotificationSettings
    {
        private readonly IUserProfileDataService _userProfileDataService;

        public InitializeNotificationSettings(IUserProfileDataService userProfileDataService)
        {
            _userProfileDataService = userProfileDataService;
        }

        public void Execute(OnUserAddedEventArgs args)
        {
            InitializeUserSettings(args.User);
        }

        public void InitializeUserSettings(User user)
        {
            if (user.Get<NotificationSettings>() == null)
            {
                _userProfileDataService.Add(new NotificationSettings
                                                {
                                                    PersistentNotificationsEnabled = true,
                                                    TransientNotificationsEnabled = true,
                                                    LastMarkedAsRead = null,
                                                    User = user
                                                });
            }
        }
    }
}