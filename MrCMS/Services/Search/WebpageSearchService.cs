﻿using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Querying;
using MrCMS.Models.Search;
using MrCMS.Paging;
using MrCMS.Settings;

namespace MrCMS.Services.Search
{
    public class WebpageSearchService : IWebpageSearchService
    {
        private readonly ISearcher<Webpage, WebpageIndexDefinition> _documentSearcher;
        private readonly IDocumentService _documentService;

        public WebpageSearchService(ISearcher<Webpage, WebpageIndexDefinition> documentSearcher, IDocumentService documentService)
        {
            _documentSearcher = documentSearcher;
            _documentService = documentService;
        }

        public IPagedList<Webpage> Search(AdminWebpageSearchQuery model)
        {
            return _documentSearcher.Search(model.GetQuery(), model.Page);
        }

        public IEnumerable<QuickSearchResults> QuickSearch(AdminWebpageSearchQuery model)
        {
            return _documentSearcher.Search(model.GetQuery(), model.Page, 10).Select(x => new QuickSearchResults
            {
                Id = x.Id,
                Name = x.Name,
                CreatedOn = x.CreatedOn.ToShortDateString().ToString(),
                Type = x.GetType().Name.ToString()
            });
        }

        public IEnumerable<Document> GetBreadCrumb(int? parentId)
        {
            return _documentService.GetParents(parentId).Reverse();
        }
    }
}