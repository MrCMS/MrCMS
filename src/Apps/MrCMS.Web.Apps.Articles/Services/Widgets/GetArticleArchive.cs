using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public class GetArticleArchive : IGetArticleArchive
    {
        private readonly IArticleArchiveService _articleArchiveService;

        public GetArticleArchive(IArticleArchiveService articleArchiveService)
        {
            _articleArchiveService = articleArchiveService;
        }

        public async Task<List<ArchiveModel>> GetArticlArchiveList(ArticleList articleList)
        {
            return await _articleArchiveService.GetMonthsAndYears(articleList);
        }
    }
}
