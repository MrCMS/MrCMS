using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Website.Auth
{
    public interface ICheckStandardAccessLogic
    {
        Task<StandardLogicCheckResult> Check(User user);
    }
}