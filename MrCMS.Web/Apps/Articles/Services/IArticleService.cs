using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Widgets;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Articles.Services
{
    public interface IArticleService
    {
        IPagedList<Article> GetArticles(ArticleList page, ArticleSearchModel model);
        List<ArchiveModel> GetMonthsAndYears(ArticleList articleList);
    }

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
