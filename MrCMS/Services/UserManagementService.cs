using System.Linq;
using MrCMS.Data;
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
        private readonly IRepository<User> _userRepository;

        public UserManagementService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public void AddUser(User user)
        {
            _userRepository.Add(user);
            EventContext.Instance.Publish<IOnUserAdded, OnUserAddedEventArgs>(new OnUserAddedEventArgs(user));
        }

        public void SaveUser(User user)
        {
            _userRepository.Update(user);
        }

        public User GetUser(int id)
        {
            return _userRepository.Get(id);
        }

        public void DeleteUser(User user)
        {
            _userRepository.Delete(user);
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
                return !_userRepository.Query().Any(u => u.Email == email && u.Id != id.Value);
            }
            return !_userRepository.Query().Any(u => u.Email == email);
        }

        public IPagedList<User> GetAllUsersPaged(int page)
        {
            return _userRepository.Query().Paged(page);
        }
    }
}