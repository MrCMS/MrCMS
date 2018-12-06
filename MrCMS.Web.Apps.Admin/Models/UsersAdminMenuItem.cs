using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class UserAdminMenuItem : IAdminMenuItem
    {
        private readonly IAccessChecker _accessChecker;

        private SubMenu _children;

        public UserAdminMenuItem(IAccessChecker accessChecker)
        {
            _accessChecker = accessChecker;
        }

        public string Text => "Users";
        public string IconClass => "fa fa-users";
        public string Url { get; private set; }
        public bool CanShow => _accessChecker.CanAccess<UserAdminMenuACL>(UserAdminMenuACL.ShowMenu);

        public SubMenu Children => _children ??
                                   (_children = GetChildren());

        public int DisplayOrder => 99;

        private SubMenu GetChildren()
        {
            return new SubMenu
            {
                new ChildMenuItem("Users", "/Admin/User",
                    _accessChecker.CanAccess<UserAdminMenuACL>(UserAdminMenuACL.Users)),
                new ChildMenuItem("Roles", "/Admin/Role",
                    _accessChecker.CanAccess<UserAdminMenuACL>(UserAdminMenuACL.Roles)),
                new ChildMenuItem("Third Party Auth", "/Admin/ThirdPartyAuth",
                    _accessChecker.CanAccess<UserAdminMenuACL>(UserAdminMenuACL.ThirdPartyAuth)),
                new ChildMenuItem("User Subscription Reports ", "/Admin/UserSubscriptionReports",
                    _accessChecker.CanAccess<UserAdminMenuACL>(UserAdminMenuACL.UserSubscriptionReports))
            };
        }
    }
}