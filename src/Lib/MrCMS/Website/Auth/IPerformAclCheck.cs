using System.Collections.Generic;

namespace MrCMS.Website.Auth
{
    public interface IPerformAclCheck
    {
        bool CanAccessLogic(StandardLogicCheckResult result, IList<string> keys);
    }
}