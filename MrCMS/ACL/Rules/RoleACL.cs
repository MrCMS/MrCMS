using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class RoleACL : ACLRule
    {
        public const string View = "View";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        public override string DisplayName
        {
            get { return "Roles"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> {View, Add, Edit, Delete};
        }
    }
}