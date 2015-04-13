using System.Web.Mvc;
using MrCMS.DbConfiguration.Filters;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Website
{
    public class EnablePublishedFilterAttribute : ActionFilterAttribute
    {
        private bool _removeOnExecuted = false;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Get<ISession>();
            if (session != null && session.GetEnabledFilter(PublishedFilter.FilterName) == null)
            {
                session.EnableFilter(PublishedFilter.FilterName);
                _removeOnExecuted = true;
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var session = filterContext.HttpContext.Get<ISession>();
            if (_removeOnExecuted)
                session.DisableFilter(PublishedFilter.FilterName);
        }
    }
}