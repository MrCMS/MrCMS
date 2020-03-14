using System;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public class WebpageUrlService : IWebpageUrlService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IRepository<Webpage> _repository;
        private readonly IRepository<PageTemplate> _pageTemplateRepository;
        private readonly IUrlValidationService _urlValidationService;

        public WebpageUrlService(
            IRepository<Webpage> repository,
            IRepository<PageTemplate> pageTemplateRepository,
            IUrlValidationService urlValidationService, IServiceProvider serviceProvider,
            IConfigurationProvider configurationProvider)
        {
            _repository = repository;
            _pageTemplateRepository = pageTemplateRepository;
            _urlValidationService = urlValidationService;
            _serviceProvider = serviceProvider;
            _configurationProvider = configurationProvider;
        }

        public async Task<string> Suggest(SuggestParams suggestParams)
        {
            var documentType = suggestParams.DocumentType;
            var parts = documentType.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                suggestParams.DocumentType = parts[0];
                suggestParams.Template = int.TryParse(parts[1], out var id) ? id : (int?)null;
            }

            IWebpageUrlGenerator generator = await GetGenerator(suggestParams.DocumentType, suggestParams.Template);
            var parent = suggestParams.ParentId.HasValue ? _repository.GetDataSync(suggestParams.ParentId.Value) : null;

            string url = generator.GetUrl(suggestParams.PageName, parent, suggestParams.UseHierarchy);

            //make sure the URL is unique

            if (!await _urlValidationService.UrlIsValidForWebpage(url, suggestParams.WebpageId))
            {
                int counter = 1;

                while (!await _urlValidationService.UrlIsValidForWebpage(string.Format("{0}-{1}", url, counter), suggestParams.WebpageId))
                    counter++;

                url = $"{url}-{counter}";
            }
            return url;
        }

        private async Task<IWebpageUrlGenerator> GetGenerator(string documentType, int? template)
        {
            IWebpageUrlGenerator generator = null;
            int id = template.GetValueOrDefault(0);
            if (id > 0)
            {
                var pageTemplate = _pageTemplateRepository.GetDataSync(id);
                Type urlGeneratorType = pageTemplate.GetUrlGeneratorType();
                if (pageTemplate != null && urlGeneratorType != null)
                {
                    generator = _serviceProvider.GetService(urlGeneratorType) as IWebpageUrlGenerator;
                }
            }
            if (generator == null && documentType != null)
            {
                var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
                generator = _serviceProvider.GetService(settings.GetGeneratorType(documentType)) as IWebpageUrlGenerator;
            }
            return generator ?? new DefaultWebpageUrlGenerator();
        }
    }
}