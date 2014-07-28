using System.Collections.Generic;
using MrCMS.ACL.Rules;
using MrCMS.Website;

namespace MrCMS.Models
{
    public class SystemAdminMenuItem : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "System"; } }
        public string Url { get; private set; }
        public bool CanShow
        {
            get { return new SystemAdminMenuACL().CanAccess(CurrentRequestData.CurrentUser, SystemAdminMenuACL.ShowMenu); }
        }

        public SubMenu Children
        {
            get
            {
                return _children ??
                       (_children = GetChildren());
            }
        }

        private static SubMenu GetChildren()
        {
            var systemAdminMenuACL = new SystemAdminMenuACL();
            return new SubMenu
                   {
                       {
                           "",
                           new List<ChildMenuItem>
                           {
                               new ChildMenuItem("Settings","#",subMenu: new SubMenu
                               {
                                   {"",new List<ChildMenuItem>
                                   {
                               new ChildMenuItem("Site Settings", "/Admin/Settings",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.SiteSettings)),
                               new ChildMenuItem("Filesystem Settings", "/Admin/Settings/FileSystem",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.FileSystemSettings)),
                               new ChildMenuItem("ACL", "/Admin/ACL",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ACL))
                                   }}
                               }),
                               new ChildMenuItem("Import/Export Documents", "/Admin/ImportExport/Documents",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ImportExport)),
                               new ChildMenuItem("Message Templates", "/Admin/MessageTemplate",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.MessageTemplates)),
                               new ChildMenuItem("Page Templates", "/Admin/PageTemplate",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.PageTemplates)),
                               new ChildMenuItem("Url Generators", "/Admin/UrlGeneratorSettings",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.UrlGenerators)),
                               new ChildMenuItem("Sites", "/Admin/Sites",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Sites)),
                               new ChildMenuItem("Resources", "/Admin/Resource",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Resources)),
                               new ChildMenuItem("Logs", "/Admin/Log",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Logs)),
                               new ChildMenuItem("Tasks", "/Admin/Task",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Tasks)),
                               new ChildMenuItem("Indexes", "/Admin/Indexes",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Indices)),
                               new ChildMenuItem("Message Queue", "/Admin/MessageQueue",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.MessageQueue)),
                               new ChildMenuItem("Notifications", "/Admin/Notification",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Notifications)),
                               new ChildMenuItem("Clear Caches", "/Admin/ClearCaches",
                                   ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ClearCaches)),
                               new ChildMenuItem("Logout", "/Logout"),
                           }
                       }
                   };
        }

        public int DisplayOrder { get { return 100; } }
    }
}