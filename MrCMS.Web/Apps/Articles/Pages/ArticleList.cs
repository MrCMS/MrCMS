using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class ArticleList : TextPage
    {
        [DisplayName("Page Size")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Page size must be a number")]
        public virtual int PageSize { get; set; }
        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }
    }

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