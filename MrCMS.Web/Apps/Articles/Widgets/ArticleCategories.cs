using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleCategories : Widget
    {
        public virtual ArticleSection ArticleSection { get; set; }
        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }

    }
    public class GetArticleCategoriesViewData : BaseAssignWidgetAdminViewData<ArticleCategories>
    {
        private readonly ISession _session;

        public GetArticleCategoriesViewData(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(ArticleCategories widget, ViewDataDictionary viewData)
        {
            viewData["ArticleLists"] = _session.QueryOver<ArticleSection>()
                                       .OrderBy(list => list.Name)
                                       .Desc.Cacheable()
                                       .List()
                                       .BuildSelectItemList(category => category.Name,
                                                            category => category.Id.ToString(),
                                                            emptyItemText: "Select an article list...");
        }
    }
}
