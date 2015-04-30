using System;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MrCMS.Website.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AntiForgeryValidationAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            try
            {
                AntiForgery.Validate();
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("~");
            }
        }
    }
}