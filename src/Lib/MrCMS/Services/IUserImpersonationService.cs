using System.Security.Claims;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services;

public interface IUserImpersonationService
{
    Task<UserImpersonationResult> Impersonate(ClaimsPrincipal currentPrincipal, User userToImpersonate);
    Task<User> GetCurrentlyImpersonatedUser(ClaimsPrincipal principal);
    Task<User> CancelImpersonation(ClaimsPrincipal principal);
}