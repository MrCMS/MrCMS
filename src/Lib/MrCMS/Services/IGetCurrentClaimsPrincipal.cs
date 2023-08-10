using System.Security.Claims;
using System.Threading.Tasks;

namespace MrCMS.Services;

public interface IGetCurrentClaimsPrincipal
{
    Task<ClaimsPrincipal> GetPrincipal();

    Task<ClaimsPrincipal> GetLoggedInClaimsPrincipal();
    Task<ClaimsPrincipal> GetImpersonatedClaimsPrincipal();
}