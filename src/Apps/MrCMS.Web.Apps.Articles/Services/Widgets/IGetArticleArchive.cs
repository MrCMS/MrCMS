using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public interface IGetArticleArchive
    {
        Task<List<ArchiveModel>> GetArticlArchiveList(ArticleList articleList);
    }
}
