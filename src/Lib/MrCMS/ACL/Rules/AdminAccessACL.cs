namespace MrCMS.ACL.Rules
{
    public class AdminAccessACL : ACLRule
    {
        public const string Allowed = "Allowed";

        public override string DisplayName => "Admin Access";

        //protected override List<string> GetOperations()
        //{
        //    return new List<string>
        //               {
        //                   Allowed
        //               };
        //}
    }
}