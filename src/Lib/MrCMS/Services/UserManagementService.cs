using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services.Events;
using MrCMS.Services.Events.Args;
using X.PagedList;

namespace MrCMS.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IGlobalRepository<User> _repository;
        private readonly IEventContext _eventContext;

        public UserManagementService(IGlobalRepository<User> repository, IEventContext eventContext)
        {
            _repository = repository;
            _eventContext = eventContext;
        }

        public async Task AddUser(User user)
        {
            await _repository.Add(user);
            await _eventContext.Publish<IOnUserAdded, OnUserAddedEventArgs>(new OnUserAddedEventArgs(user));
        }

        public async Task SaveUser(User user)
        {
            await _repository.Update(user);
        }

        public Task<User> GetUser(int id)
        {
            return _repository.Load(id);
        }

        public async Task DeleteUser(int id)
        {
            var user = await GetUser(id);
            if (user == null)
                return;
            await _repository.Delete(user);
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
                return !await _repository.Query().AnyAsync(u => u.Email == email && u.Id != id.Value);
            }

            return !await _repository.Query().AnyAsync(u => u.Email == email);
        }

        public Task<IPagedList<User>> GetAllUsersPaged(int page)
        {
            return _repository.Query().ToPagedListAsync(page, 10);
        }
    }
}