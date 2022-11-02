using System.Security.Claims;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services;

public interface IUserImpersonationService
{
    Task<UserImpersonationResult> Impersonate(ClaimsPrincipal currentPrincipal, User user);
    Task<User> GetCurrentlyImpersonatedUser(ClaimsPrincipal principal);
    Task CancelImpersonation(ClaimsPrincipal principal);
}