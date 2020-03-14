using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Widgets;


namespace MrCMS.Web.Apps.Articles.Services.Widgets
{
    public class GetLatestXArticlesModel : GetWidgetModelBase<LatestXArticles>
    {
        private readonly IRepository<Article> _repository;

        public GetLatestXArticlesModel(IRepository<Article> repository)
        {
            _repository = repository;
        }

        public override async Task<object> GetModel(LatestXArticles widget)
        {
            if (widget.RelatedNewsList == null)
                return null;

            return new LatestXArticlesViewModel
            {
                Articles = await _repository.Readonly()
                    .Where(article => article.ParentId == widget.RelatedNewsList.Id
                                      && article.PublishOn != null && article.PublishOn <= DateTime.UtcNow)
                    .OrderByDescending(x => x.PublishOn)
                    .Take(widget.NumberOfArticles)
                    .ToListAsync(),
                Title = widget.Name
            };
        }
    }
}
