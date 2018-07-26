using System.Collections.Generic;
using MrCMS.Apps;

namespace MrCMS.ACL.Rules
{
    public class SiteSettingsACL : ACLRule
    {
        public const string View = "View";
        public const string Save = "Save";

        public override string DisplayName
        {
            get { return "Site Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                      {
                          View,
                          Save
                      };
        }
    }
}