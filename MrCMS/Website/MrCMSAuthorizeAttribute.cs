using MrCMS.ACL.Rules;

namespace MrCMS.Website
{
    public class MrCMSAuthorizeAttribute : MrCMSBaseAuthorizationAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var currentUser = CurrentRequestData.CurrentUser;
            return base.AuthorizeCore(httpContext) && currentUser != null &&
                   currentUser.IsActive &&
                   currentUser.Email == httpContext.User.Identity.Name &&
                   currentUser.CanAccess<AdminAccessACL>("Allowed");
        }
    }
}