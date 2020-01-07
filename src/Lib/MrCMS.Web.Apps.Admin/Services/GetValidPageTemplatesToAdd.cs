using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class GetValidPageTemplatesToAdd : IGetValidPageTemplatesToAdd
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<PageTemplate> _pageTemplateRepository;

        public GetValidPageTemplatesToAdd(IRepository<Webpage> webpageRepository, IRepository<PageTemplate> pageTemplateRepository)
        {
            _webpageRepository = webpageRepository;
            _pageTemplateRepository = pageTemplateRepository;
        }

        public async Task<List<PageTemplate>> Get(IEnumerable<DocumentMetadata> validWebpageDocumentTypes)
        {
            List<string> typeNames = validWebpageDocumentTypes.Select(metadata => metadata.Type.FullName).ToList();

            var templates = await _pageTemplateRepository.Readonly().Where(template => typeNames.Contains(template.PageType))
                .OrderBy(template => template.Name).ToListAsync();

            templates = templates.FindAll(template =>
            {
                if (!template.SingleUse)
                    return true;
                return !_webpageRepository.Readonly().Any(webpage => webpage.PageTemplate.Id == template.Id);
            });
            return templates;
        }
    }
}