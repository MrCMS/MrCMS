using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Widgets;

namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public class GetArticleArchiveModel : GetWidgetModelBase<ArticleArchive>
    {
        private readonly IArticleService _articleService;

        public GetArticleArchiveModel(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public override object GetModel(ArticleArchive widget)
        {
            var model = new ArticleArchiveModel
            {
                ArticleYearsAndMonths = _articleService.GetMonthsAndYears(widget.ArticleList),
                ArticleList = widget.ArticleList,
                ArticleArchive = widget
            };

            return model;
        }
    }
}