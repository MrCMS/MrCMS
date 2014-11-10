using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class SystemAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string SiteSettings = "Site Settings";
        public const string FileSystemSettings = "File System Settings";
        public const string Sites = "Sites";
        public const string Resources = "Resources";
        public const string Logs = "Logs";
        public const string Batch = "Batch";
        public const string Tasks = "Tasks";
        public const string ImportExport = "Import/Export";
        public const string MessageTemplates = "Message Templates";
        public const string PageTemplates = "Page Templates";
        public const string UrlGenerators = "URL Generators";
        public const string ACL = "ACL";
        public const string Indices = "Indices";
        public const string MessageQueue = "Message Queue";
        public const string Notifications = "Notifications";
        public const string ClearCaches = "Clear Caches";

        public override string DisplayName
        {
            get { return "System Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
                   {
                       ShowMenu,
                       SiteSettings,
                       FileSystemSettings,
                       Sites,
                       Resources,
                       Logs,
                       Batch,
                       Tasks,
                       ImportExport,
                       MessageTemplates,
                       PageTemplates,
                       UrlGenerators,
                       ACL,
                       Indices,
                       MessageQueue,
                       Notifications,
                       ClearCaches
                   };
        }
    }
}