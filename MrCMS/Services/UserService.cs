using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class UserService : IUserService
    {
        private readonly ISession _session;

        public UserService(ISession session)
        {
            _session = session;
        }

        public void SaveUser(User user)
        {
            _session.Transact(session => session.SaveOrUpdate(user));
        }

        public User GetUser(int id)
        {
            return _session.Get<User>(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _session.QueryOver<User>().Cacheable().List();
        }

        public User GetUserByEmail(string email)
        {
            return
                _session.QueryOver<User>().Where(user => user.Email.IsLike(email, MatchMode.Exact)).Cacheable().
                    SingleOrDefault();
        }

        public User GetUserByResetGuid(Guid resetGuid)
        {
            return
                _session.QueryOver<User>()
                    .Where(user => user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= DateTime.UtcNow)
                    .Cacheable().SingleOrDefault();
        }

        public User GetCurrentUser(HttpContextBase context)
        {
            return context.User != null ? GetUserByEmail(context.User.Identity.Name) : null;
        }
    }
}