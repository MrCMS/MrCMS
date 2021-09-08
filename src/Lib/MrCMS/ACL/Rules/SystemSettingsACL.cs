namespace MrCMS.ACL.Rules
{
    public class SystemSettingsACL : ACLRule
    {
        public const string View = "View";
        public const string Save = "Save";

        public override string DisplayName => "System Settings";

        //protected override List<string> GetOperations()
        //{
        //    return new List<string>
        //    {
        //        View,
        //        Save
        //    };
        //}
    }
}