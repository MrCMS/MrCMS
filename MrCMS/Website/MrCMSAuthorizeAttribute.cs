using System.Web.Mvc;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class MrCMSAuthorizeAttribute : AuthorizeAttribute
    {
        IUserService userService { get { return MrCMSApplication.Get<IUserService>(); } }
        
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var user = userService.GetUserByEmail(httpContext.User.Identity.Name);
            return base.AuthorizeCore(httpContext) && user != null && user.IsActive;
        }
    }
}