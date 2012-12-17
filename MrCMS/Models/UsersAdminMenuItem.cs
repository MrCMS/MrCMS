using System.Collections.Generic;

namespace MrCMS.Models
{
    public class UsersAdminMenuItem : IAdminMenuItem
    {
        private List<IMenuItem> _children;
        public string Text { get { return "Users"; } }
        public string Url { get; private set; }
        public List<IMenuItem> Children { get
        {
            return _children ??
                   (_children =
                    new List<IMenuItem>
                        {
                            new ChildMenuItem("Users", "/Admin/User"),
                            new ChildMenuItem("Roles", "/Admin/Role")
                        });
        } }
        public int DisplayOrder { get { return -1; } }
    }
}