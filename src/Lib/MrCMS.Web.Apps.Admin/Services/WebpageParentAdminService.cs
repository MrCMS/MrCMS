using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class WebpageParentAdminService : IWebpageParentAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;

        public WebpageParentAdminService(IRepository<Webpage> webpageRepository)
        {
            _webpageRepository = webpageRepository;
        }

        public Webpage GetWebpage(int id)
        {
            return _webpageRepository.LoadSync(id);
        }

        public IEnumerable<SelectListItem> GetValidParents(Webpage webpage)
        {
            List<DocumentMetadata> validParentTypes = DocumentMetadataHelper.GetValidParentTypes(webpage);

            List<string> validParentTypeNames =
                validParentTypes.Select(documentMetadata => documentMetadata.Type.FullName).ToList();
            IList<Webpage> potentialParents =
                _webpageRepository.Query()
                    .Where(page => validParentTypeNames.Contains(page.DocumentType))
                    .ToList();

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

        public void Set(int id, int? parentVal)
        {
            var webpage = GetWebpage(id);

            if (webpage == null)
                return;

            Webpage parent = parentVal.HasValue ? _webpageRepository.LoadSync(parentVal.Value) : null;

            webpage.Parent = parent;

            _webpageRepository.Update(webpage);
        }
    }
}