using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Widgets;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetArticleArchiveAdminViewData:BaseAssignWidgetAdminViewData<ArticleArchive>
    {
        private readonly ISession _session;

        public GetArticleArchiveAdminViewData(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(ArticleArchive widget, ViewDataDictionary viewData)
        {
            viewData["ArticleLists"] = _session.QueryOver<ArticleSection>()
                .OrderBy(list => list.Name)
                .Asc.Cacheable()
                .List()
                .BuildSelectItemList(category => category.Name,
                    category => category.Id.ToString(),
                    emptyItemText: "Select an article list...");
        }
    }
}