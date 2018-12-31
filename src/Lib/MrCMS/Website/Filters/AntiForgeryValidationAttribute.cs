using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AntiForgeryValidationAttribute : Attribute, /*FilterAttribute, */IAuthorizationFilter
    {
        //public void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    if (filterContext == null)
        //    {
        //        throw new ArgumentNullException("filterContext");
        //    }
        //    try
        //    {
        //        AntiForgery.Validate();
        //    }
        //    catch 
        //    {
        //        filterContext.Result = new RedirectResult("~");
        //    }
        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // TODO: anti-forgery
            //throw new NotImplementedException();
        }
    }
}