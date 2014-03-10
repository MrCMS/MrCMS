using System;
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
            int monthVal;
            int? month = int.TryParse(Convert.ToString(GetValueFromContext(controllerContext, "month")), out monthVal)
                ? monthVal
                : (int?) null;
            int yearVal;
            int? year = int.TryParse(Convert.ToString(GetValueFromContext(controllerContext, "year")), out yearVal)
                ? yearVal
                : (int?) null;
            return new ArticleSearchModel
                       {
                           Page = page,
                           Category = GetValueFromContext(controllerContext, "category"),
                           Month = month,
                           Year = year
                       };
        }
    }
}