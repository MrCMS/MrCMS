using MrCMS.Entities.People;
using MrCMS.Entities.UserProfile;
using MrCMS.Services;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class NotificationSettingsAdminService : INotificationSettingsAdminService
    {
        private readonly IInitializeNotificationSettings _initializeNotificationSettings;
        private readonly IUserProfileDataService _userProfileDataService;

        public NotificationSettingsAdminService(IInitializeNotificationSettings initializeNotificationSettings, IUserProfileDataService userProfileDataService)
        {
            _initializeNotificationSettings = initializeNotificationSettings;
            _userProfileDataService = userProfileDataService;
        }

        public NotificationSettings InitializeSettings(User user)
        {
            _initializeNotificationSettings.InitializeUserSettings(user);
            return user.Get<NotificationSettings>();
        }

        public void Update(NotificationSettings settings)
        {
            _userProfileDataService.Update(settings);
        }
    }
}