using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class SystemSettingsACL : ACLRule
    {
        public const string View = "View";
        public const string Save = "Save";

        public override string DisplayName
        {
            get { return "System Settings"; }
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