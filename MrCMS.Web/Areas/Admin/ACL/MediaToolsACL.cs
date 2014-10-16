using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Areas.Admin.ACL
{
    public class MediaToolsACL : ACLRule
    {
        public const string Delete = "Delete";
        public const string Cut = "Cut";

        public override string DisplayName
        {
            get { return "Media Tools"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                Delete, Cut
            };
        }
    }
}