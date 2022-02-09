using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services.Events;
using MrCMS.Services.Events.Args;
using NHibernate;
using X.PagedList;

namespace MrCMS.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ISession _session;
        private readonly IEventContext _eventContext;

        public UserManagementService(ISession session, IEventContext eventContext)
        {
            _session = session;
            _eventContext = eventContext;
        }

        public async Task AddUser(User user)
        {
            await _session.TransactAsync((session) => session.SaveAsync(user));
            await _eventContext.Publish<IOnUserAdded, OnUserAddedEventArgs>(new OnUserAddedEventArgs(user));
        }

        public async Task SaveUser(User user)
        {
            await _session.TransactAsync(session => session.UpdateAsync(user));
        }

        public Task<User> GetUser(int id)
        {
            return _session.GetAsync<User>(id);
        }

        public async Task DeleteUser(int id)
        {
            var user = await GetUser(id);
            if (user == null)
                return;
            await _session.TransactAsync((session) => session.DeleteAsync(user));
        }

        /// <summary>
        ///     Checks to see if the supplied email address is unique
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id">The id of user to exclude from check. Has to be string because of AdditionFields on Remote property</param>
        /// <returns></returns>
        public async Task<bool> IsUniqueEmail(string email, int? id = null)
        {
            if (id.HasValue)
            {
                return await _session.QueryOver<User>().Where(u => u.Email == email && u.Id != id.Value)
                    .RowCountAsync() == 0;
            }

            return await _session.QueryOver<User>().Where(u => u.Email == email).RowCountAsync() == 0;
        }

        public Task<IPagedList<User>> GetAllUsersPaged(int page)
        {
            return _session.QueryOver<User>().PagedAsync(page);
        }
    }
}