using System;
using System.ComponentModel;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleCategories : Widget
    {
        public virtual ArticleList ArticleList { get; set; }
        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }

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


        public virtual string Category { get { return CurrentRequestData.CurrentContext.Request["category"]; } }
    }

}
