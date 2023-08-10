using System.Security.Claims;
using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public interface ICheckStandardAccessLogic
    {
        StandardLogicCheckResult Check(ClaimsPrincipal user);
    }
}