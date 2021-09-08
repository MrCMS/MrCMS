namespace MrCMS.ACL.Rules
{
    public class InlineEditingACL : ACLRule
    {
        public const string Allowed = "Allowed";

        public override string DisplayName => "Inline Editing";

        //protected override List<string> GetOperations()
        //{
        //    return new List<string> { Allowed };
        //}
    }
}