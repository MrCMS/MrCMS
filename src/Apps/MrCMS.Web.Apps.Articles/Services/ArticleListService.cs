using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;

using NHibernate.Criterion;
using X.PagedList;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class ArticleListService : IArticleListService
    {
        private readonly ISession _session;

        public ArticleListService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Article> GetArticles(ArticleList page, ArticleSearchModel model)
        {
            var query = _session.QueryOver<Article>()
                .Where(a => a.Parent == page && a.Published);

            if (!string.IsNullOrEmpty(model.Category))
            {
                Tag tagAlias = null;
                query = query.JoinAlias(article => article.Tags, () => tagAlias).Where(() => tagAlias.Name.IsInsensitiveLike(model.Category, MatchMode.Exact));
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

            return query.OrderBy(x => x.PublishOn).Desc.Paged(model.Page, page.PageSize);
        }
    }
}