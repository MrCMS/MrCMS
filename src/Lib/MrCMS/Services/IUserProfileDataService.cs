using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserProfileDataService
    {
        Task<T> Get<T>(int id) where T : UserProfileData;
        Task Add<T>(T data) where T : UserProfileData;
        Task Update<T>(T data) where T : UserProfileData;
        Task Delete<T>(T data) where T : UserProfileData;
    }
}