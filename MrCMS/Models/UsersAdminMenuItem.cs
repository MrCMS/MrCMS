using System.Collections.Generic;
using System.Web;
using MrCMS.ACL.Rules;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Models
{
    public class UserAdminMenuItem : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Users"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return new UserAdminMenuACL().CanAccess(CurrentRequestData.CurrentUser, UserAdminMenuACL.ShowMenu); }
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
            var userAdminMenuACL = new UserAdminMenuACL();
            return new SubMenu
                       {
                           {
                               "",
                               new List<ChildMenuItem>
                                   {
                                       new ChildMenuItem("Users", "/Admin/User",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.Users)),
                                       new ChildMenuItem("Roles", "/Admin/Role",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.Roles)),
                                       new ChildMenuItem("Your Account",
                                                         "/Admin/User/Edit/" +
                                                         CurrentRequestData.CurrentUser.Id,
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.YourAccount)),
                                       new ChildMenuItem("Third Party Auth", "/Admin/ThirdPartyAuth",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.ThirdPartyAuth)),
                                   }
                           }
                       };
        }

        public int DisplayOrder { get { return 99; } }
    }
}