using System.Collections.Generic;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleArchiveService
    {
        List<ArchiveModel> GetMonthsAndYears(ArticleList articleList);
    }
}
