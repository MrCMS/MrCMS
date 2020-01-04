using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Indexing.Definitions;
using MrCMS.Indexing.Querying;
using MrCMS.Indexing.Utils;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Admin.Models.Search;

using NHibernate.Criterion;
using X.PagedList;
using Document = MrCMS.Entities.Documents.Document;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class AdminWebpageSearchService : IAdminWebpageSearchService
    {
        private readonly ISearcher<Webpage, AdminWebpageIndexDefinition> _documentSearcher;
        private readonly IGetBreadcrumbs _getBreadcrumbs;
        private readonly IGetLiveUrl _getLiveUrl;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IStringResourceProvider _stringResourceProvider;

        public AdminWebpageSearchService(ISearcher<Webpage, AdminWebpageIndexDefinition> documentSearcher,
            IGetBreadcrumbs getBreadcrumbs, ISession session, Site site, IStringResourceProvider stringResourceProvider,
            IGetLiveUrl getLiveUrl)
        {
            _documentSearcher = documentSearcher;
            _getBreadcrumbs = getBreadcrumbs;
            _session = session;
            _site = site;
            _stringResourceProvider = stringResourceProvider;
            _getLiveUrl = getLiveUrl;
        }

        public IPagedList<Webpage> Search(AdminWebpageSearchQuery model)
        {
            return _documentSearcher.Search(GetQuery(model), model.Page);
        }

        public IEnumerable<QuickSearchResult> QuickSearch(AdminWebpageSearchQuery model)
        {
            return Enumerable.Select(_documentSearcher.Search(GetQuery(model), model.Page, 10), x =>
                new QuickSearchResult
                {
                    id = x.Id,
                    value = x.Name,
                    url = _getLiveUrl.GetAbsoluteUrl(x)
                });
        }


        public IEnumerable<Document> GetBreadCrumb(int? parentId)
        {
            return _getBreadcrumbs.Get(parentId).Reverse();
        }

        public List<SelectListItem> GetDocumentTypes(string type)
        {
            return DocumentMetadataHelper.DocumentMetadatas
                .BuildSelectItemList(definition => definition.Name, definition => definition.TypeName,
                    definition => definition.TypeName == type,
                    _stringResourceProvider.GetValue("Admin Select Type", "Select type"));
        }

        public List<SelectListItem> GetParentsList()
        {
            var parentIds = _session.QueryOver<Webpage>()
                .Where(webpage => webpage.Parent != null)
                .SelectList(
                    builder =>
                        builder.Select(
                            Projections.Distinct(Projections.Property<Webpage>(webpage => webpage.Parent.Id))))
                .Cacheable()
                .List<int>().ToList();
            var rootWebpages = _session.QueryOver<Webpage>()
                .Where(webpage => webpage.Parent == null && webpage.Site.Id == _site.Id && webpage.Id.IsIn(parentIds))
                .OrderBy(webpage => webpage.DisplayOrder)
                .Asc.List();
            var selectListItems =
                GetPageListItems(
                    rootWebpages, parentIds, 1).ToList();
            selectListItems.Insert(0,
                new SelectListItem
                {
                    Selected = false,
                    Text = _stringResourceProvider.GetValue("Admin Root", "Root"),
                    Value = "0"
                });
            return selectListItems;
        }

        public Query GetQuery(AdminWebpageSearchQuery model)
        {
            if (string.IsNullOrWhiteSpace(model.Term) && string.IsNullOrWhiteSpace(model.Type) &&
                !model.CreatedOnTo.HasValue && !model.CreatedOnFrom.HasValue &&
                model.ParentId == null) return new MatchAllDocsQuery();

            var booleanQuery = new BooleanQuery();
            if (!string.IsNullOrWhiteSpace(model.Term))
                booleanQuery.Add(model.Term.GetSearchFilterByTerm(_documentSearcher.Definition.SearchableFieldNames));
            if (model.CreatedOnFrom.HasValue || model.CreatedOnTo.HasValue)
                booleanQuery.Add(GetDateQuery(model), Occur.MUST);

            if (!string.IsNullOrEmpty(model.Type))
                booleanQuery.Add(
                    new TermQuery(new Term(_documentSearcher.Definition.GetFieldDefinition<TypeFieldDefinition>()?.Name,
                        model.Type)),
                    Occur.MUST);

            if (model.ParentId != null)
                booleanQuery.Add(
                    new TermQuery(new Term(
                        _documentSearcher.Definition.GetFieldDefinition<ParentIdFieldDefinition>().Name,
                        model.ParentId.ToString())), Occur.MUST);

            return booleanQuery;
        }

        private Query GetDateQuery(AdminWebpageSearchQuery model)
        {
            return new TermRangeQuery(_documentSearcher.Definition.GetFieldDefinition<CreatedOnFieldDefinition>().Name,
                model.CreatedOnFrom.HasValue
                    ? new BytesRef(DateTools.DateToString(model.CreatedOnFrom.Value, DateTools.Resolution.SECOND))
                    : null,
                model.CreatedOnTo.HasValue
                    ? new BytesRef(DateTools.DateToString(model.CreatedOnTo.Value, DateTools.Resolution.SECOND))
                    : null, model.CreatedOnFrom.HasValue, model.CreatedOnTo.HasValue);
        }

        private IEnumerable<SelectListItem> GetPageListItems(IEnumerable<Webpage> pages, List<int> parentIds, int depth)
        {
            var items = new List<SelectListItem>();

            foreach (var node in pages.Where(node => parentIds.Contains(node.Id)))
            {
                items.Add(new SelectListItem
                {
                    Selected = false,
                    Text = GetDashes(depth) + node.Name,
                    Value = node.Id.ToString()
                });
                items.AddRange(GetPageListItems(_session.QueryOver<Webpage>()
                        .Where(webpage => webpage.Parent.Id == node.Id && webpage.Id.IsIn(parentIds))
                        .OrderBy(webpage => webpage.DisplayOrder)
                        .Asc.Cacheable()
                        .List(), parentIds, depth + 1));
            }

            return items;
        }

        private string GetDashes(int depth)
        {
            return string.Empty.PadRight(depth * 2, '-');
        }
    }
}