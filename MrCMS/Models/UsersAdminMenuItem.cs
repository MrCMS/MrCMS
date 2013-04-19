using System.Collections.Generic;
using System.Web;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Models
{
    public class UserAdminMenuItem : ISystemAdminMenuItem
    {
        private Dictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "Users"; } }
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
                                                        new ChildMenuItem("Users", "/Admin/User"),
                                                        new ChildMenuItem("Roles", "/Admin/Role"),
                                                        new ChildMenuItem("Your Account",
                                                                          "/Admin/User/Edit/" +
                                                                          CurrentRequestData.CurrentUser.Id)
                                                    }
                                            }
                                        });
            }
        }
        public int DisplayOrder { get { return 99; } }
    }
}