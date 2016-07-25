using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ISession _session;

        public ArticleService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Article> GetArticles(ArticleList page, ArticleSearchModel model)
        {
            var query = _session.QueryOver<Article>()
                                .Where(a => a.Parent == page);

            if (!String.IsNullOrEmpty(model.Category))
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

        public List<ArchiveModel> GetMonthsAndYears(ArticleList articleList)
        {
            var query = (from article in _session.Query<Article>()
                         where article.Parent == articleList && article.PublishOn != null
                         group article by new { article.PublishOn.Value.Year, article.PublishOn.Value.Month } into entryGroup
                         select new ArchiveModel
                                    {
                                        Date = new DateTime(entryGroup.Key.Year, entryGroup.Key.Month, 1),
                                        Count = entryGroup.Count()
                                    });
            return query.ToList().OrderByDescending(x => x.Date).ToList();

        }
    }
}