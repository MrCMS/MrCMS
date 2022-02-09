using MrCMS.Web.Apps.Articles.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public interface IGetLatestXArticles
    {
        Task<IList<Article>> GetArticlesAsync(int? relatedNewsId, int numberOfArticles);
    }
}
