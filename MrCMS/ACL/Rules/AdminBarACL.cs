using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class AdminBarACL : ACLRule
    {
        public const string Show = "Show";

        public override string DisplayName
        {
            get { return "Admin Bar"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Show };
        }
    }
}