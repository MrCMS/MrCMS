using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class SystemAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string SiteSettings = "Site Settings";
        public const string FileSystemSettings = "File System Settings";
        public const string Sites = "Sites";
        public const string Logs = "Logs";
        public const string Tasks = "Tasks";
        public const string ImportExport = "Import/Export";
        public const string ACL = "ACL";
        public const string Indices = "Indices";

        public override string DisplayName
        {
            get { return "System Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { ShowMenu, SiteSettings, FileSystemSettings, Sites, Logs, Tasks, ImportExport, ACL, Indices };
        }
    }

    public class UserAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string YourAccount = "Your Account";
        public override string DisplayName
        {
            get { return "User Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { ShowMenu, Users, Roles, YourAccount };
        }
    }
}