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
        public const string Impersonate = "Impersonate";
        public const string EditAdmin = "Edit Admin";
        public override string DisplayName => "Users";
    }
}