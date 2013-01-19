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
                            new ChildMenuItem("Users", "/Admin/User"),
                            new ChildMenuItem("Roles", "/Admin/Role"),
                            new ChildMenuItem("Sites", "/Admin/Sites"),
                        });
            }
        }
        public int DisplayOrder { get { return 100; } }
    }

    public interface ISystemAdminMenuItem : IAdminMenuItem
    {
    }
}