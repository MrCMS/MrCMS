using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class AdminWebpageSearchService : IAdminWebpageSearchService
    {
        private readonly ISearcher<Webpage, AdminWebpageIndexDefinition> _documentSearcher;
        private readonly IDocumentService _documentService;

        public AdminWebpageSearchService(ISearcher<Webpage, AdminWebpageIndexDefinition> documentSearcher, IDocumentService documentService)
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