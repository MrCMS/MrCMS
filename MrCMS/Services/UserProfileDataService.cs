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

        public void Add<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Add(data);
            _session.Transact(session => session.Save(data));
        }

        public void Update<T>(T data) where T : UserProfileData
        {
            _session.Transact(session => session.Update(data));
        }

        public void Delete<T>(T data) where T : UserProfileData
        {
            var user = data.User;
            if (user != null) user.UserProfileData.Remove(data);
            _session.Transact(session => session.Delete(data));
        }
    }
}