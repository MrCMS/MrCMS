using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleArchiveService
    {
        Task<List<ArchiveModel>> GetMonthsAndYears(ArticleList articleList);
    }
}
