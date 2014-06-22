using System;
using System.Linq;
using System.ServiceModel.Syndication;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.ActionResults;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Controllers
{
    public class ArticleRSSController : MrCMSAppUIController<ArticlesApp>
    {
        private readonly IArticlesRssService _articlesRssService;

        public ArticleRSSController(IArticlesRssService articlesRssService)
        {
            _articlesRssService = articlesRssService;
        }

        public RssActionResult Show(ArticleList page)
        {
            var feed = _articlesRssService.GetSyndicationFeed(page);
            return new RssActionResult { Feed = feed };
        }
    }

    public interface IArticlesRssService
    {
        SyndicationFeed GetSyndicationFeed(ArticleList page);
    }

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