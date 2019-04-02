using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using X.PagedList;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleListService
    {
        IPagedList<Article> GetArticles(ArticleList page, ArticleSearchModel model);
    }
}
