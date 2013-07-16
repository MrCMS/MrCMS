using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class IndexACL : ACLRule
    {
        public const string View = "View";
        public const string Reindex = "Reindex";
        public const string Create = "Create";
        public const string Optimize = "Optimize";
        public override string DisplayName
        {
            get { return "Indices"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                       {
                           View,
                           Reindex,
                           Create,
                           Optimize
                       };
        }
    }
}