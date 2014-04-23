using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Commenting.ACL
{
    public class CommentingAdminACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string EditSettings = "Edit Settings";
        public const string ViewComments = "View Comments";

        public override string DisplayName
        {
            get { return "Comment Admin"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                       {
                           ShowMenu,
                           EditSettings,
                           ViewComments
                       };
        }
    }
}