using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class AclAdminACL : ACLRule
    {
        public const string View = "View";
        public const string Edit = "Edit";

        public override string DisplayName
        {
            get { return "ACLAdmin"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View, Edit };
        }
    }
}