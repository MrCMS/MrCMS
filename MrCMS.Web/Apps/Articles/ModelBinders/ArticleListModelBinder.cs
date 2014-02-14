using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.ModelBinders
{
    public class ArticleListModelBinder : MrCMSDefaultModelBinder
    {
        public ArticleListModelBinder(ISession session)
            : base(() => session)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int page;
            int.TryParse(GetValueFromContext(controllerContext, "page"), out page);
            return new ArticleSearchModel
                       {
                           Page = page,
                           Category = GetValueFromContext(controllerContext, "category")
                       };
        }
    }
}