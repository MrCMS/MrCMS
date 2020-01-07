using System.Linq;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;

using X.PagedList;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class ArticleListService : IArticleListService
    {
        private readonly IRepository<Article> _repository;

        public ArticleListService(IRepository<Article> repository)
        {
            _repository = repository;
        }

        public IPagedList<Article> GetArticles(ArticleList page, ArticleSearchModel model)
        {
            var query = _repository.Query()
                .Where(a => a.Parent == page && a.Published);

            if (!string.IsNullOrEmpty(model.Category))
            {
                query = query.Where(x => x.DocumentTags.Any(y => EF.Functions.Like(y.Tag.Name, model.Category)));
            }

            if (model.Month.HasValue)
            {
                query =
                    query.Where(
                        article => article.PublishOn != null && article.PublishOn.Value.Month == model.Month);
            }
            if (model.Year.HasValue)
            {
                query =
                    query.Where(
                        article => article.PublishOn != null && article.PublishOn.Value.Year == model.Year);
            }

            return query.OrderByDescending(x => x.PublishOn).ToPagedList(model.Page, page.PageSize);
        }
    }
}