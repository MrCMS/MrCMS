using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate.Linq;

namespace MrCMS.Web.Admin.Services
{
    public class WebpageParentAdminService : IWebpageParentAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IWebpageMetadataService _webpageMetadataService;

        public WebpageParentAdminService(IRepository<Webpage> webpageRepository,
            IWebpageMetadataService webpageMetadataService)
        {
            _webpageRepository = webpageRepository;
            _webpageMetadataService = webpageMetadataService;
        }

        public async Task<Webpage> GetWebpage(int id)
        {
            return await _webpageRepository.Get(id);
        }

        public async Task<IEnumerable<SelectListItem>> GetValidParents(Webpage webpage)
        {
            List<WebpageMetadata> validParentTypes = _webpageMetadataService.GetValidParentTypes(webpage);

            List<string> validParentTypeNames =
                validParentTypes.Select(documentMetadata => documentMetadata.Type.FullName).ToList();
            IList<Webpage> potentialParents =
                await _webpageRepository.Query()
                    .Where(page => validParentTypeNames.Contains(page.WebpageType))
                    .ToListAsync();

            List<SelectListItem> result = potentialParents.Distinct()
                .Where(page => !page.ActivePages.Contains(webpage))
                .OrderBy(x => x.Name)
                .BuildSelectItemList(
                    page => $"{page.Name} ({_webpageMetadataService.GetMetadata(page).Name})",
                    page => page.Id.ToString(),
                    webpage1 => webpage.Parent != null && webpage.Parent.Id == webpage1.Id, emptyItem: null);

            if (!_webpageMetadataService.GetMetadata(webpage).RequiresParent)
                result.Insert(0, SelectListItemHelper.EmptyItem("Root"));

            return result;
        }

        public async Task Set(int id, int? parentVal)
        {
            var webpage = await GetWebpage(id);

            if (webpage == null)
                return;

            Webpage parent = parentVal.HasValue ? await _webpageRepository.Get(parentVal.Value) : null;

            webpage.Parent = parent;

            await _webpageRepository.Update(webpage);
        }
    }
}