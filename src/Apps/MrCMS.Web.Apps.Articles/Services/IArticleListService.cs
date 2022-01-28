using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using System.Threading.Tasks;
using X.PagedList;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleListService
    {
        Task<IPagedList<Article>> GetArticlesAsync(ArticleList page, ArticleSearchModel model);
    }
}
