using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Querying;
using MrCMS.Indexing.Utils;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Indexing;
using MrCMS.Web.Apps.Core.Indexing.WebpageSearch;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using Document = MrCMS.Entities.Documents.Document;

namespace MrCMS.Web.Apps.Core.Services.Search
{
    public class WebpageSearchService : IWebpageSearchService
    {
        private readonly ISearcher<Webpage, WebpageSearchIndexDefinition> _documentSearcher;
        private readonly IGetBreadcrumbs _getBreadcrumbs;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public WebpageSearchService(ISearcher<Webpage, WebpageSearchIndexDefinition> documentSearcher,
            IGetBreadcrumbs getBreadcrumbs, 
            IGetDateTimeNow getDateTimeNow)
        {
            _documentSearcher = documentSearcher;
            _getBreadcrumbs = getBreadcrumbs;
            _getDateTimeNow = getDateTimeNow;
        }

        public async Task<IPagedList<Webpage>> Search(WebpageSearchQuery model)
        {
            return await _documentSearcher.Search(await GetQuery(model), model.Page);
        }

        public IEnumerable<Document> GetBreadCrumb(int? parentId)
        {
            return _getBreadcrumbs.Get(parentId).Reverse();
        }
        public async Task<Query> GetQuery(WebpageSearchQuery model)
        {
            var booleanQuery = new BooleanQuery
            {
                {
                    new TermRangeQuery(
                        _documentSearcher.Definition.GetFieldDefinition<PublishedOnFieldDefinition>().Name, null,
                        new BytesRef( DateTools.DateToString(await _getDateTimeNow.GetLocalNow(), DateTools.Resolution.SECOND)), false, true),
                    Occur.MUST
                }
            };
            if (!string.IsNullOrWhiteSpace(model.Term))
            {
                var indexDefinition = _documentSearcher.Definition;
                booleanQuery.Add(model.Term.GetSearchFilterByTerm(indexDefinition.SearchableFieldNames));
            }

            if (model.CreatedOnFrom.HasValue || model.CreatedOnTo.HasValue)
            {
                booleanQuery.Add(GetDateQuery(model), Occur.MUST);
            }

            if (!string.IsNullOrEmpty(model.Type))
            {
                booleanQuery.Add(new TermQuery(new Term(_documentSearcher.Definition.GetFieldDefinition<TypeFieldDefinition>().Name, model.Type)),
                    Occur.MUST);
            }

            if (model.ParentId != null)
            {
                var definition = _documentSearcher.Definition.GetFieldDefinition<ParentIdFieldDefinition>();
                booleanQuery.Add(new TermQuery(new Term(definition.Name, model.ParentId.ToString())), Occur.MUST);
            }

            return booleanQuery;
        }

        private Query GetDateQuery(WebpageSearchQuery model)
        {
            var definition = _documentSearcher.Definition.GetFieldDefinition<CreatedOnFieldDefinition>();
            return new TermRangeQuery(definition.Name,
                model.CreatedOnFrom.HasValue
                    ? new BytesRef(DateTools.DateToString(model.CreatedOnFrom.Value, DateTools.Resolution.SECOND))
                    : null,
                model.CreatedOnTo.HasValue
                    ? new BytesRef(DateTools.DateToString(model.CreatedOnTo.Value, DateTools.Resolution.SECOND))
                    : null, model.CreatedOnFrom.HasValue, model.CreatedOnTo.HasValue);
        }
    }
}