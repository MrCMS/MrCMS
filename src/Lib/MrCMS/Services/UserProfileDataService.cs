using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class UserProfileDataService : IUserProfileDataService
    {
        private readonly IRepositoryResolver _repositoryResolver;

        public UserProfileDataService(IRepositoryResolver repositoryResolver)
        {
            _repositoryResolver = repositoryResolver;
        }

        public Task<T> Get<T>(int id) where T : UserProfileData
        {
            return _repositoryResolver.GetGlobalRepository<T>().Load(id);
        }

        public async Task Add<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Add(data);
            await _repositoryResolver.GetGlobalRepository<T>().Add(data);
        }

        public async Task Update<T>(T data) where T : UserProfileData
        {
            await _repositoryResolver.GetGlobalRepository<T>().Update(data);
        }

        public async Task Delete<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Remove(data);
            await _repositoryResolver.GetGlobalRepository<T>().Delete(data);
        }
    }
}