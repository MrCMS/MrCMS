using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class UserAdminMenuItem : IAdminMenuItem
    {
        private readonly IGetCurrentUser _getCurrentUser;

        public UserAdminMenuItem(IGetCurrentUser getCurrentUser)
        {
            _getCurrentUser = getCurrentUser;
        }

        private SubMenu _children;
        public string Text { get { return "Users"; } }
        public string IconClass { get { return "fa fa-users"; } }
        public string Url { get; private set; }
        public bool CanShow
        {
            get { return new UserAdminMenuACL().CanAccess(_getCurrentUser.Get(), UserAdminMenuACL.ShowMenu); }
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