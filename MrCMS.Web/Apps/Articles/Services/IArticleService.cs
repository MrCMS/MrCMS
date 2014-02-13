using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Paging;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Widgets;
using MrCMS.Website;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleService
    {
        IPagedList<Article> GetArticles(ArticleList page, ArticleSearchModel model);
        List<ArchiveModel> GetMonthsAndYears(ArticleList articleList);
    }
}
