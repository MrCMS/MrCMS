using System;
using System.Linq;
using System.ServiceModel.Syndication;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class ArticlesRssService : IArticlesRssService
    {
        private readonly ISession _session;

        public ArticlesRssService(ISession session)
        {
            _session = session;
        }

        public SyndicationFeed GetSyndicationFeed(ArticleList page)
        {
            var possiblyPublishedArticles =
                _session.QueryOver<Article>()
                    .Where(article => article.Parent.Id == page.Id && article.PublishOn != null)
                    .Cacheable()
                    .List().Where(article => article.Published).OrderByDescending(article => article.PublishOn);

            var items = possiblyPublishedArticles.Select(GetItem).ToList();

            return new SyndicationFeed(page.Name, page.BodyContent.StripHtml(), new Uri(page.AbsoluteUrl), items);
        }

        private SyndicationItem GetItem(Article article)
        {
            return new SyndicationItem(article.Name, article.Abstract.StripHtml(),
                new Uri(article.AbsoluteUrl))
            {
                PublishDate = new DateTimeOffset(article.PublishOn.Value)
            };
        }
    }
}