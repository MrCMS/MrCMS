using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class ArticleArchiveService : IArticleArchiveService
    {
        private readonly ISession _session;

        public ArticleArchiveService(ISession session)
        {
            _session = session;
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