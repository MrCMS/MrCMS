using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class TopXArticles : Widget
    {
        public virtual int NumberOfArticles { get; set; }
        public virtual ArticleList RelatedNewsList { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            if (RelatedNewsList == null)
                return null;

            return
                session.QueryOver<Article>()
                       .Where(article => article.Parent.Id == RelatedNewsList.Id && article.Published)
                       .Take(NumberOfArticles)
                       .Cacheable()
                       .List();
        }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, NHibernate.ISession session)
        {
            viewData["newsList"] = session.QueryOver<ArticleList>()
                                                .Where(article => article.Published)
                                                .Cacheable()
                                                .List()
                                                .BuildSelectItemList(item => item.Name,
                                                                     item => item.Id.ToString(),
                                                                     emptyItemText: "Please select news list");
        }
    }
    
}