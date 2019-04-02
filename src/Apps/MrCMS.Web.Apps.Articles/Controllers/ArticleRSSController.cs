//using MrCMS.Web.Apps.Articles.Pages;
//using MrCMS.Website.Controllers;

//namespace MrCMS.Web.Apps.Articles.Controllers
//{
//    public class ArticleRSSController : MrCMSAppUIController<MrCMSArticlesApp>
//    {
//        private readonly IArticlesRssService _articlesRssService;

//        public ArticleRSSController(IArticlesRssService articlesRssService)
//        {
//            _articlesRssService = articlesRssService;
//        }

//        public RssActionResult Show(ArticleList page)
//        {
//            var feed = _articlesRssService.GetSyndicationFeed(page);
//            return new RssActionResult { Feed = feed };
//        }
//    }
//}