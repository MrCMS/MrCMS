namespace MrCMS.ACL.Rules
{
    public class AclAdminACL : ACLRule
    {
        public const string View = "View";
        public const string Edit = "Edit";

        public override string DisplayName => "ACLAdmin";

        //protected override List<string> GetOperations()
        //{
        //    return new List<string> { View, Edit };
        //}
    }
}