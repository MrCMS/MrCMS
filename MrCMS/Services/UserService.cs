using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class UserService : IUserService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public UserService(ISession session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
        }

        public void AddUser(User user)
        {
            _session.Transact(session =>
                                  {
                                      session.Save(user);
                                  });
        }

        public void SaveUser(User user)
        {
            _session.Transact(session => session.Update(user));
        }

        public User GetUser(int id)
        {
            return _session.Get<User>(id);
        }

        public IPagedList<User> GetAllUsersPaged(int page)
        {
            return _session.QueryOver<User>().Paged(page, _siteSettings.DefaultPageSize);
        }

        public User GetUserByEmail(string email)
        {
            string trim = email.Trim();
            return _session.QueryOver<User>().Where(user => user.Email == trim).Take(1).Cacheable().SingleOrDefault();
        }

        public User GetUserByResetGuid(Guid resetGuid)
        {
            return
                _session.QueryOver<User>()
                    .Where(user => user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= CurrentRequestData.Now)
                    .Cacheable().SingleOrDefault();
        }

        public User GetCurrentUser(HttpContextBase context)
        {
            return context.User != null ? GetUserByEmail(context.User.Identity.Name) : null;
        }

        public void DeleteUser(User user)
        {
            _session.Transact(session =>
                                  {
                                      user.OnDeleting(session);
                                      session.Delete(user);
                                  });
        }

        /// <summary>
        /// Checks to see if the supplied email address is unique
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id">The id of user to exlcude from check. Has to be string because of AdditionFields on Remote property</param>
        /// <returns></returns>
        public bool IsUniqueEmail(string email, int? id = null)
        {
            if (id.HasValue)
            {
                return _session.QueryOver<User>().Where(u => u.Email == email && u.Id != id.Value).RowCount() == 0;
            }
            return _session.QueryOver<User>().Where(u => u.Email == email).RowCount() == 0;
        }

        public T Get<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _session.QueryOver<T>().Where(arg => arg.User == user).Take(1).Cacheable().SingleOrDefault();
        }

        public IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _session.QueryOver<T>().Where(arg => arg.User == user).Cacheable().List();
        }

        public IPagedList<T> GetPaged<T>(User user, QueryOver<T> query = null, int page = 1) where T : SystemEntity, IBelongToUser
        {
            return _session.Paged(query ?? QueryOver.Of<T>(), page, _siteSettings.DefaultPageSize);
        }
    }
}