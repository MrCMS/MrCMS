using System;
using System.Collections.Generic;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class LatestXArticles : Widget
    {
        public virtual int NumberOfArticles { get; set; }
        public virtual ArticleSection RelatedNewsSection { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            if (RelatedNewsSection == null)
                return null;


            return new LatestXArticlesViewModel
                       {
                           Articles = session.QueryOver<Article>()
                                           .Where(article => article.Parent.Id == RelatedNewsSection.Id && article.PublishOn != null && article.PublishOn <= CurrentRequestData.Now)
                                           .OrderBy(x => x.PublishOn).Desc
                                           .Take(NumberOfArticles)
                                           .Cacheable()
                                           .List(),
                           Title = this.Name
                       };

        }

        public override void SetDropdownData(System.Web.Mvc.ViewDataDictionary viewData, NHibernate.ISession session)
        {
            viewData["newsList"] = session.QueryOver<ArticleSection>()
                                                .Where(article => article.PublishOn != null && article.PublishOn <= CurrentRequestData.Now)
                                                .Cacheable()
                                                .List()
                                                .BuildSelectItemList(item => item.Name,
                                                                     item => item.Id.ToString(),
                                                                     emptyItemText: "Please select news list");
        }
    }

    public class LatestXArticlesViewModel
    {
        public IList<Article> Articles { get; set; }
        public string Title { get; set; }
    }

}