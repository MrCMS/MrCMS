using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class UserAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string YourAccount = "Your Account";
        public override string DisplayName
        {
            get { return "User Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { ShowMenu, Users, Roles, YourAccount };
        }
    }
}