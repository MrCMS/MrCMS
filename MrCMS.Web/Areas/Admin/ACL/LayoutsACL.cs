using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Areas.Admin.ACL
{
    public class LayoutsACL : ACLRule
    {
        public const string Show = "Show";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
        public const string Sort = "Sort";

        public override string DisplayName
        {
            get { return "Layouts"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Show, Add, Edit, Delete, Sort };
        }
    }
}