using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserProfileDataService
    {
        void Add<T>(T data) where T : UserProfileData;
        void Update<T>(T data) where T : UserProfileData;
        void Delete<T>(T data) where T : UserProfileData;
    }
}