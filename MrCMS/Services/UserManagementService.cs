using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services.Events;
using MrCMS.Services.Events.Args;
using NHibernate;

namespace MrCMS.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ISession _session;

        public UserManagementService(ISession session)
        {
            _session = session;
        }

        public void AddUser(User user)
        {
            _session.Transact(session => { session.Save(user); });
            EventContext.Instance.Publish<IOnUserAdded, OnUserAddedEventArgs>(new OnUserAddedEventArgs(user));
        }

        public void SaveUser(User user)
        {
            _session.Transact(session => session.Update(user));
        }

        public User GetUser(int id)
        {
            return _session.Get<User>(id);
        }

        public void DeleteUser(User user)
        {
            _session.Transact(session => session.Delete(user));
        }

        /// <summary>
        ///     Checks to see if the supplied email address is unique
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id">The id of user to exclude from check. Has to be string because of AdditionFields on Remote property</param>
        /// <returns></returns>
        public bool IsUniqueEmail(string email, int? id = null)
        {
            if (id.HasValue)
            {
                return _session.QueryOver<User>().Where(u => u.Email == email && u.Id != id.Value).RowCount() == 0;
            }
            return _session.QueryOver<User>().Where(u => u.Email == email).RowCount() == 0;
        }

        public IPagedList<User> GetAllUsersPaged(int page)
        {
            return _session.QueryOver<User>().Paged(page);
        }
    }
}