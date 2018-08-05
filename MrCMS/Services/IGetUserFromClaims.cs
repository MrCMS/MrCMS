using System.Security.Claims;
using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IGetUserFromClaims
    {
        Task<User> GetUserAsync(ClaimsPrincipal principal);
    }
}