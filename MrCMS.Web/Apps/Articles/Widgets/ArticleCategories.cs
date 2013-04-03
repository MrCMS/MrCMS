using System;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleCategories : Widget
    {
        public virtual ArticleList ArticleList { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            if (ArticleList != null)
                return ArticleList;

            return session.QueryOver<ArticleList>().Cacheable().List().FirstOrDefault();
        }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, NHibernate.ISession session)
        {
            viewData["ArticleLists"] = session.QueryOver<ArticleList>()
                                       .OrderBy(list => list.Name)
                                       .Desc.Cacheable()
                                       .List()
                                       .BuildSelectItemList(category => category.Name,
                                                            category => category.Id.ToString(),
                                                            emptyItemText: "Select an article list...");
        }

    }
}
