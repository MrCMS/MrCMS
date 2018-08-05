using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserLoginManager
    {
        Task<User> FindByLoginAsync(string loginProvider, string providerKey);
        Task<IdentityResult> RemoveLoginAsync(User user, string loginProvider, string providerKey);
        Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo login);
        Task<IList<UserLoginInfo>> GetLoginsAsync(User user);
    }
}