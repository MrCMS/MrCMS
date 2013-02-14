using System.Web.Mvc;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class MrCMSAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext) && CurrentRequestData.CurrentUser != null &&
                   CurrentRequestData.CurrentUser.IsActive &&
                   CurrentRequestData.CurrentUser.Email == httpContext.User.Identity.Name;
        }
    }
}