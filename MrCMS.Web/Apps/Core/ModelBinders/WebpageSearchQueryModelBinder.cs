using System.Web.Mvc;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Core.ModelBinders
{
    public class WebpageSearchQueryModelBinder : MrCMSDefaultModelBinder
    {
        public WebpageSearchQueryModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int page;
            int.TryParse(GetValueFromContext(controllerContext, "page"), out page);
            if (page == 0)
                page = 1;
            return new WebpageSearchQuery
            {
                Page = page,
                Term = GetValueFromContext(controllerContext, "term")
            };
        }
    }
}