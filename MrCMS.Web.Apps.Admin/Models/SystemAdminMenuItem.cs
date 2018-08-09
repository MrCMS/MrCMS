using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class SystemAdminMenuItem : IAdminMenuItem
    {
        private readonly IAccessChecker _accessChecker;

        public SystemAdminMenuItem(IAccessChecker accessChecker)
        {
            _accessChecker = accessChecker;
        }

        private SubMenu _children;
        public string Text => "System";
        public string IconClass => "fa fa-cogs";
        public string Url { get; private set; }

        public bool CanShow => _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.ShowMenu);

        public SubMenu Children => _children ??
                                   (_children = GetChildren());

        public int DisplayOrder => 100;

        private SubMenu GetChildren()
        {
            return new SubMenu
            {
                new ChildMenuItem("Settings", "#",_accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.ShowMenu),  new SubMenu
                {
                    new ChildMenuItem("Site Settings", "/Admin/Settings", _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.SiteSettings)),
                    new ChildMenuItem("System Settings", "/Admin/SystemSettings",
                        _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.SystemSettings)),
                    new ChildMenuItem("Filesystem Settings", "/Admin/Settings/FileSystem",
                        _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.FileSystemSettings)),
                    new ChildMenuItem("Mail Settings", "/Admin/SystemSettings/Mail",
                        _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.SiteSettings)),
                    new ChildMenuItem("ACL", "/Admin/ACL",
                        _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.ACL))
                }),
                new ChildMenuItem("Security", "#", _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Security),
                    new SubMenu
                    {
                        new ChildMenuItem("Custom Scripts", "/Admin/CustomScriptPages",
                            _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Security)),
                        new ChildMenuItem("Security Options", "/Admin/SecurityOptions",
                            _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Security))
                    }),
                new ChildMenuItem("Import/Export Documents", "/Admin/ImportExport/Documents",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.ImportExport)),
                new ChildMenuItem("Message Templates", "/Admin/MessageTemplate",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.MessageTemplates)),
                new ChildMenuItem("Page Templates", "/Admin/PageTemplate",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.PageTemplates)),
                new ChildMenuItem("Page Defaults", "/Admin/PageDefaults",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.UrlGenerators)),
                new ChildMenuItem("Sites", "/Admin/Sites",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Sites)),
                new ChildMenuItem("Resources", "/Admin/Resource",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Resources)),
                new ChildMenuItem("Logs", "/Admin/Log",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Logs)),
                new ChildMenuItem("Batches", "/Admin/Batch",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Batch)),
                new ChildMenuItem("Tasks", "/Admin/Task",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Tasks)),
                new ChildMenuItem("Indexes", "/Admin/Indexes",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Indices)),
                new ChildMenuItem("Message Queue", "/Admin/MessageQueue",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.MessageQueue)),
                new ChildMenuItem("Notifications", "/Admin/Notification",
                    _accessChecker.CanAccess<SystemAdminMenuACL>(SystemAdminMenuACL.Notifications)),
                new ChildMenuItem("Clear Caches", "/Admin/ClearCaches"),
                new ChildMenuItem("About", "/Admin/About")
            };
        }
    }
}