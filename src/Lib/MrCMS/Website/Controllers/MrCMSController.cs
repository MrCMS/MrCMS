using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Website.Filters;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Website.Controllers
{

    [ReturnUrlHandler(Order = 999)]
    public abstract class MrCMSController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CheckCurrentSite(filterContext);
        }

        private void CheckCurrentSite(ActionExecutingContext filterContext)
        {
            List<SiteEntity> entities = filterContext.ActionArguments.Values.OfType<SiteEntity>().ToList();

            var site = filterContext.HttpContext.RequestServices.GetRequiredService<Site>();
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