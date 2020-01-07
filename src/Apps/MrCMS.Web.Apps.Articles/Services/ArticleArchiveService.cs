using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;


namespace MrCMS.Web.Apps.Articles.Services
{
    public class ArticleArchiveService : IArticleArchiveService
    {
        private readonly IRepository<Article> _repository;

        public ArticleArchiveService(IRepository<Article> repository)
        {
            _repository = repository;
        }

        public List<ArchiveModel> GetMonthsAndYears(ArticleList articleList)
        {
            var query = (from article in _repository.Readonly()
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