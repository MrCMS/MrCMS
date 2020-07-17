using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Admin.ACL.UserSubscriptionReports
{
    public class UserSubscriptionReportsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "UserSubscriptionReports"; }
        }

        //protected override List<string> GetOperations()
        //{
        //    return new List<string>
        //           {
        //               View
        //           };
        //}
    }
}