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
        public string IconClass { get { return "fa fa-users"; } }
        public string Url { get; private set; }
        public bool CanShow
        {
            get { return new UserAdminMenuACL().CanAccess(CurrentRequestData.CurrentUser, UserAdminMenuACL.ShowMenu); }
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
                           
                              
                                       new ChildMenuItem("Users", "/Admin/User",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.Users)),
                                       new ChildMenuItem("Roles", "/Admin/Role",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.Roles)),
                                       new ChildMenuItem("Third Party Auth", "/Admin/ThirdPartyAuth",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.ThirdPartyAuth)),
                                       new ChildMenuItem("User Subscription Reports ", "/Admin/UserSubscriptionReports",
                                                         ACLOption.Create(userAdminMenuACL, UserAdminMenuACL.UserSubscriptionReports))                     
                       };
        }

        public int DisplayOrder { get { return 99; } }
    }
}