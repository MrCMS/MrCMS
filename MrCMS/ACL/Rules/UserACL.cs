using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class UserACL : ACLRule
    {
        public const string View = "View";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
        public const string SetPassword = "Set Password";
        public const string Roles = "Edit User Roles";

        public override string DisplayName
        {
            get { return "Users"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View, Add, Edit, Delete, SetPassword, Roles };
        }
    }
}