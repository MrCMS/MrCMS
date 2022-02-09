using MrCMS.ACL;

namespace MrCMS.Web.Admin.ACL.UserSubscriptionReports
{
    public class UserRegistrationReportsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "UserSubscriptionReports"; }
        }
    }
}