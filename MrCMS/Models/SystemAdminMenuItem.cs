using System.Collections.Generic;

namespace MrCMS.Models
{
    public class SystemAdminMenuItem : ISystemAdminMenuItem
    {
        private Dictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "System"; } }
        public string Url { get; private set; }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ??
                       (_children = new Dictionary<string, List<IMenuItem>>
                                        {
                                            {
                                                "",
                                                new List<IMenuItem>
                                                    {
                                                        new ChildMenuItem("Settings", "/Admin/Settings"),
                                                        new ChildMenuItem("Sites", "/Admin/Sites"),
                                                        new ChildMenuItem("Logs", "/Admin/Log"),
                                                        new ChildMenuItem("Tasks", "/Admin/Task"),
                                                        new ChildMenuItem("Indexes", "/Admin/Indexes"),
                                                        new ChildMenuItem("Logout", "/Logout"),
                                                    }
                                            }
                                        });
            }
        }
        public int DisplayOrder { get { return 100; } }
    }
}