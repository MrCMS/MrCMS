using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Indexes;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class ArticleAdminService : IArticleAdminService
    {
        private readonly ISearcher<Article, ArticleIndexDefinition> _articleSearcher;
        private readonly IGetArticleSectionOptions _getArticleSectionOptions;

        public ArticleAdminService(ISearcher<Article, ArticleIndexDefinition> articleSearcher,
            IGetArticleSectionOptions getArticleSectionOptions)
        {
            _articleSearcher = articleSearcher;
            _getArticleSectionOptions = getArticleSectionOptions;
        }

        public IPagedList<Article> Search(ArticleSearchQuery query)
        {
            var booleanQuery = new BooleanQuery();
            if (query.SectionId.HasValue)
            {
                booleanQuery.Add(
                    new TermQuery(new Term(FieldDefinition.GetFieldName<ArticleSectionsDefinition>(),
                        query.SectionId.ToString())), Occur.MUST);
            }

            var sort = new Sort(new[]
            {
                new SortField(FieldDefinition.GetFieldName<ArticlePublishedDefinition>(),
                    SortField.STRING, true)
            });
            Query luceneQuery = booleanQuery.Clauses.Any() ? (Query) booleanQuery : new MatchAllDocsQuery();
            return _articleSearcher.Search(luceneQuery, query.Page, sort: sort);
        }

        public List<SelectListItem> GetArticleSectionOptions()
        {
            return _getArticleSectionOptions.GetOptions();
        }

        public List<SelectListItem> GetPrimarySectionOptions()
        {
            List<SelectListItem> articleSectionOptions = GetArticleSectionOptions();
            articleSectionOptions.Insert(0, SelectListItemHelper.EmptyItem("Any"));
            return articleSectionOptions;
        }
    }
}