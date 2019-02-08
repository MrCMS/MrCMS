using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserProfileDataService
    {
        T Get<T>(int id) where T : UserProfileData;
        void Add<T>(T data) where T : UserProfileData;
        void Update<T>(T data) where T : UserProfileData;
        void Delete<T>(T data) where T : UserProfileData;
    }
}