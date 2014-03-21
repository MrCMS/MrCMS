using MrCMS.Entities.People;
using MrCMS.Entities.UserProfile;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface INotificationSettingsAdminService
    {
        NotificationSettings InitializeSettings(User user);
        void Update(NotificationSettings settings);
    }
}