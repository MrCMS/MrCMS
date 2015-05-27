using MrCMS.ACL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Areas.Admin.ACL.UserSubscriptionReports
{
    public class UserSubscriptionReportsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "UserSubscriptionReports"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                   {
                       View
                   };
        }
    }
}