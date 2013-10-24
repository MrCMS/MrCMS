using System.Collections.Generic;
using MrCMS.ACL.Rules;
using MrCMS.Website;

namespace MrCMS.Models
{
    public class SystemAdminMenuItem : ISystemAdminMenuItem
    {
        private Dictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "System"; } }
        public string Url { get; private set; }
        public bool CanShow
        {
            get { return new SystemAdminMenuACL().CanAccess(CurrentRequestData.CurrentUser, SystemAdminMenuACL.ShowMenu); }
        }

        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ??
                       (_children = GetChildren());
            }
        }

        private static Dictionary<string, List<IMenuItem>> GetChildren()
        {
            var systemAdminMenuACL = new SystemAdminMenuACL();
            return new Dictionary<string, List<IMenuItem>>
                       {
                           {
                               "",
                               new List<IMenuItem>
                                   {
                                       new ChildMenuItem("Site Settings", "/Admin/Settings",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.SiteSettings)),
                                       new ChildMenuItem("Filesystem Settings", "/Admin/Settings/FileSystem",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.FileSystemSettings)),
                                       new ChildMenuItem("Import/Export Documents", "/Admin/ImportExport/Documents",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ImportExport)),
                                       new ChildMenuItem("Message Templates", "/Admin/MessageTemplate",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.MessageTemplates)),
                                       new ChildMenuItem("Sites", "/Admin/Sites",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Sites)),
                                       new ChildMenuItem("Logs", "/Admin/Log",ACLOption.Create(systemAdminMenuACL,SystemAdminMenuACL.Logs)),
                                       new ChildMenuItem("Tasks", "/Admin/Task",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Tasks)),
                                       new ChildMenuItem("ACL", "/Admin/ACL",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ACL)),
                                       new ChildMenuItem("Indices", "/Admin/Indexes",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Indices)),
                                       new ChildMenuItem("Message Queue", "/Admin/MessageQueue",ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.MessageQueue)),
                                       new ChildMenuItem("Logout", "/Logout"),
                                   }
                           }
                       };
        }

        public int DisplayOrder { get { return 100; } }
    }
}