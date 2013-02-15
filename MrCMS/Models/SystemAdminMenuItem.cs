using System.Collections.Generic;

namespace MrCMS.Models
{
    public class SystemAdminMenuItem : ISystemAdminMenuItem
    {
        private List<IMenuItem> _children;
        public string Text { get { return "System"; } }
        public string Url { get; private set; }
        public List<IMenuItem> Children
        {
            get
            {
                return _children ??
                       (_children =
                        new List<IMenuItem>
                            {
                                new ChildMenuItem("Settings", "/Admin/Settings"),
                                new ChildMenuItem("Sites", "/Admin/Sites"),
                                new ChildMenuItem("Logs", "/Admin/Log"),
                                new ChildMenuItem("Tasks", "/Admin/Task"),
                                new ChildMenuItem("Indexes", "/Admin/Indexes"),
                                new ChildMenuItem("Logout", "/Logout"),
                            });
            }
        }
        public int DisplayOrder { get { return 100; } }
    }
}