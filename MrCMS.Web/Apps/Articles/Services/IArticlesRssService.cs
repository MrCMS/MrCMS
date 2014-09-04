using System.ServiceModel.Syndication;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticlesRssService
    {
        SyndicationFeed GetSyndicationFeed(ArticleList page);
    }
}