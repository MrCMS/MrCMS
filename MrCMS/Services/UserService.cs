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
        private readonly IAuthorisationService _authorisationService;
        private readonly ISession _session;

        public UserService(ISession session, IAuthorisationService authorisationService = null)
        {
            _session = session ?? MrCMSApplication.Get<ISession>();
            _authorisationService = authorisationService ?? MrCMSApplication.Get<IAuthorisationService>();
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

        public User GetCurrentUser(HttpContext context)
        {
            return context.User != null ? GetUserByEmail(context.User.Identity.Name) : null;
        }

        public void SetPassword(int userId, string password)
        {
            var user = GetUser(userId);

            _authorisationService.SetPassword(user, password, password);

            SaveUser(user);
        }

        public void SaveRole(UserRole role)
        {
            _session.Transact(session => session.SaveOrUpdate(role));
        }

        public UserRole GetRole(int id)
        {
            return _session.Get<UserRole>(id);
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _session.QueryOver<UserRole>().Cacheable().List();
        }

        public UserRole GetRoleByName(string name)
        {
            return
                _session.QueryOver<UserRole>().Where(role => role.Name.IsLike(name, MatchMode.Exact)).Cacheable().
                    SingleOrDefault();
        }

        public void DeleteRole(UserRole role)
        {
            _session.Transact(session => session.Delete(role));
        }
    }
}