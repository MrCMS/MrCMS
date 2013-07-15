using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class AdminAccessACL : ACLRule
    {
        public override string DisplayName
        {
            get { return "Admin Access"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                       {
                           "Allowed"
                       };
        }
    }
}