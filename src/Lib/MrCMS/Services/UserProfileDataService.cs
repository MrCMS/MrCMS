using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class UserProfileDataService : IUserProfileDataService
    {
        private readonly ISession _session;

        public UserProfileDataService(ISession session)
        {
            _session = session;
        }

        public async Task<T> Get<T>(int id) where T : UserProfileData
        {
            return await _session.GetAsync<T>(id);
        }

        public async Task Add<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Add(data);
            await _session.TransactAsync(session => session.SaveAsync(data));
        }

        public async Task Update<T>(T data) where T : UserProfileData
        {
            await _session.TransactAsync(session => session.UpdateAsync(data));
        }

        public async Task Delete<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Remove(data);
            await _session.TransactAsync(session => session.DeleteAsync(data));
        }
    }
}