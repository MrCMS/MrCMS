namespace MrCMS.ACL.Rules
{
    public class UserAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string ThirdPartyAuth = "Third Party Auth";
        public const string UserSubscriptionReports = "UserSubscriptionReports";
        public const string YourAccount = "Your Account";
        public override string DisplayName => "User Admin Menu";

        //protected override List<string> GetOperations()
        //{
        //    return new List<string> { ShowMenu, Users, Roles, YourAccount, ThirdPartyAuth, UserSubscriptionReports };
        //}
    }
}