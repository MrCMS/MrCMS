using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Website.Auth
{
    public interface IPerformAclCheck
    {
        Task<bool> CanAccessLogic(StandardLogicCheckResult result, IList<string> keys);
    }
}