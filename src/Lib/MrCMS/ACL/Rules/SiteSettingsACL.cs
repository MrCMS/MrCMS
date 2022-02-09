namespace MrCMS.ACL.Rules
{
    public class SiteSettingsACL : ACLRule
    {
        public const string View = "View";
        public const string Save = "Save";

        public override string DisplayName => "Site Settings";

        //protected override List<string> GetOperations()
        //{
        //    return new List<string>
        //              {
        //                  View,
        //                  Save
        //              };
        //}
    }
}