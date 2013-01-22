using System.Collections.Generic;
using System.Web;
using MrCMS.Services;
using MrCMS.Website;

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
                            new ChildMenuItem("Logout", "/Logout"),
                        });
            }
        }
        public int DisplayOrder { get { return 100; } }
    }

    public class UserAdminMenuItem : ISystemAdminMenuItem
    {
        private List<IMenuItem> _children;
        public string Text { get { return "Users"; } }
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
                            new ChildMenuItem("Your Account", "/Admin/User/Edit/" + MrCMSApplication.Get<IUserService>().GetCurrentUser(MrCMSApplication.CurrentContext).Id)
                        });
            }
        }
        public int DisplayOrder { get { return 99; } }
    }
    
    public interface ISystemAdminMenuItem : IAdminMenuItem
    {
    }
}