using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Website.Filters;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    [ReturnUrl]
    public abstract class MrCMSController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CheckCurrentSite(filterContext);
        }

        private void CheckCurrentSite(ActionExecutingContext filterContext)
        {
            List<SiteEntity> entities = filterContext.ActionArguments.Values.OfType<SiteEntity>().ToList();

            var locator = filterContext.HttpContext.RequestServices.GetService<ICurrentSiteLocator>();
            if (locator == null)
                return;
            var site = locator.GetCurrentSite();
            if (entities.Any(entity => !site.IsValidForSite(entity) && entity.Id != 0) ||
                entities.Any(entity => entity.IsDeleted))
            {
                filterContext.Result = AuthenticationFailureRedirect();
            }
        }

        protected virtual RedirectResult AuthenticationFailureRedirect()
        {
            return Redirect("~");
        }
    }
}