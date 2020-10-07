using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Web.Admin.Services
{
    public class WebpageParentAdminService : IWebpageParentAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IDocumentMetadataService _documentMetadataService;

        public WebpageParentAdminService(IRepository<Webpage> webpageRepository,
            IDocumentMetadataService documentMetadataService)
        {
            _webpageRepository = webpageRepository;
            _documentMetadataService = documentMetadataService;
        }

        public Webpage GetWebpage(int id)
        {
            return _webpageRepository.Get(id);
        }

        public IEnumerable<SelectListItem> GetValidParents(Webpage webpage)
        {
            List<DocumentMetadata> validParentTypes = _documentMetadataService.GetValidParentTypes(webpage);

            List<string> validParentTypeNames =
                validParentTypes.Select(documentMetadata => documentMetadata.Type.FullName).ToList();
            IList<Webpage> potentialParents =
                _webpageRepository.Query()
                    .Where(page => validParentTypeNames.Contains(page.DocumentType))
                    .ToList();

            List<SelectListItem> result = potentialParents.Distinct()
                .Where(page => !page.ActivePages.Contains(webpage))
                .OrderBy(x => x.Name)
                .BuildSelectItemList(
                    page => string.Format("{0} ({1})", page.Name, _documentMetadataService.GetMetadata(page).Name),
                    page => page.Id.ToString(),
                    webpage1 => webpage.Parent != null && webpage.ParentId == webpage1.Id, emptyItem: null);

            if (!_documentMetadataService.GetMetadata(webpage).RequiresParent)
                result.Insert(0, SelectListItemHelper.EmptyItem("Root"));

            return result;
        }

        public void Set(int id, int? parentVal)
        {
            var webpage = GetWebpage(id);

            if (webpage == null)
                return;

            Webpage parent = parentVal.HasValue ? _webpageRepository.Get(parentVal.Value) : null;

            webpage.Parent = parent;

            _webpageRepository.Update(webpage);
        }
    }
}