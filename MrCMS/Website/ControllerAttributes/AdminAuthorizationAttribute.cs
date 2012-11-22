using System.Web.Mvc;

namespace MrCMS.Website.ControllerAttributes
{
    public class AdminAuthorizationAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            base.OnAuthorization(filterContext);
        }
    }
}