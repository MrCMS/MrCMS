using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserLockoutManager
    {
        Task<bool> IsLockedOutAsync(User user);
        Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled);
        Task<bool> GetLockoutEnabledAsync(User user);
        Task<DateTimeOffset?> GetLockoutEndDateAsync(User user);
        Task<IdentityResult> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd);
        Task<IdentityResult> AccessFailedAsync(User user);
        Task<IdentityResult> ResetAccessFailedCountAsync(User user);
        Task<int> GetAccessFailedCountAsync(User user);
    }
}