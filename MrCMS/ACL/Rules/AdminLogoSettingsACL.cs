using System.Collections.Generic;
using MrCMS.Apps;

namespace MrCMS.ACL.Rules
{
    public class AdminLogoSettingsACL : ACLRule
    {
        public const string View = "View";
        public const string Save = "Save";

        public override string DisplayName
        {
            get { return "Admin Logo Settings"; }
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
