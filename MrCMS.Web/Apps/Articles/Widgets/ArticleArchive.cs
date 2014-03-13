using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleArchive : Widget
    {
        public virtual ArticleList ArticleList { get; set; }

        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }

        public override object GetModel(ISession session)
        {
            var articleService = MrCMSApplication.Get<IArticleService>();
            var model = new ArticleArchiveModel
            {
                ArticleYearsAndMonths = articleService.GetMonthsAndYears(ArticleList),
                ArticleList = ArticleList,
                ArticleArchive = this
            };

            return model;
        }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, NHibernate.ISession session)
        {
            viewData["ArticleLists"] = session.QueryOver<ArticleList>()
                                       .OrderBy(list => list.Name)
                                       .Asc.Cacheable()
                                       .List()
                                       .BuildSelectItemList(category => category.Name,
                                                            category => category.Id.ToString(),
                                                            emptyItemText: "Select an article list...");
        }
    }

    public class ArticleArchiveModel
    {
        public ArticleList ArticleList { get; set; }
        public ArticleArchive ArticleArchive { get; set; }
        public IList<ArchiveModel> ArticleYearsAndMonths { get; set; }
        public string Year { get { return CurrentRequestData.CurrentContext.Request["year"]; } }
        public string Month { get { return CurrentRequestData.CurrentContext.Request["year"]; } }
    }
}
