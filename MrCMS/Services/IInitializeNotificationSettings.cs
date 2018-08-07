using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IInitializeNotificationSettings
    {
        void InitializeUserSettings(User user);
    }
}