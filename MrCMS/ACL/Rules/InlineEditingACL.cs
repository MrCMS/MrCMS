using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class InlineEditingACL : ACLRule
    {
        public const string Allowed = "Allowed";

        public override string DisplayName
        {
            get { return "Inline Editing"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Allowed };
        }
    }
}