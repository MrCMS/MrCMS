using System.Collections.Generic;

namespace MrCMS.Website.Auth
{
    public class StandardLogicCheckResult
    {
        public bool? CanAccess { get; set; }
        public ISet<int> Roles { get; set; }
    }
}