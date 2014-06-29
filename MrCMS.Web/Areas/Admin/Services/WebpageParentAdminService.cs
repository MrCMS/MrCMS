using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageParentAdminService : IWebpageParentAdminService
    {
        private readonly IDocumentService _documentService;
        private readonly ISession _session;

        public WebpageParentAdminService(IDocumentService documentService, ISession session)
        {
            _documentService = documentService;
            _session = session;
        }

        public IEnumerable<SelectListItem> GetValidParents(Webpage webpage)
        {
            List<DocumentMetadata> validParentTypes = DocumentMetadataHelper.GetValidParentTypes(webpage);

            List<string> validParentTypeNames =
                validParentTypes.Select(documentMetadata => documentMetadata.Type.FullName).ToList();
            IList<Webpage> potentialParents =
                _session.QueryOver<Webpage>()
                    .Where(page => page.DocumentType.IsIn(validParentTypeNames) )
                    .Cacheable().List<Webpage>();

            List<SelectListItem> result = potentialParents.Distinct()
                .Where(page => !page.ActivePages.Contains(webpage))
                .OrderBy(x => x.Name)
                .BuildSelectItemList(page => string.Format("{0} ({1})", page.Name, page.GetMetadata().Name),
                    page => page.Id.ToString(),
                    webpage1 => webpage.Parent != null && webpage.ParentId == webpage1.Id, emptyItem: null);

            if (!webpage.GetMetadata().RequiresParent)
                result.Insert(0, SelectListItemHelper.EmptyItem("Root"));

            return result;
        }

        public void Set(Webpage webpage, int? parentVal)
        {
            if (webpage == null) return;

            Webpage parent = parentVal.HasValue ? _session.Get<Webpage>(parentVal.Value) : null;

            webpage.Parent = parent;

            _documentService.SaveDocument(webpage);
        }
    }
}