using System;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Settings;
using NHibernate;
using Ninject;

namespace MrCMS.Services
{
    public class WebpageUrlService : IWebpageUrlService
    {
        private readonly PageDefaultsSettings _settings;
        private readonly IRepository<PageTemplate> _pageTemplateRepository;
        private readonly IUrlValidationService _urlValidationService;
        private IKernel _kernel;

        public WebpageUrlService(IRepository<PageTemplate> pageTemplateRepository, IUrlValidationService urlValidationService, 
            PageDefaultsSettings settings, IKernel kernel)
        {
            _pageTemplateRepository = pageTemplateRepository;
            _urlValidationService = urlValidationService;
            _settings = settings;
            _kernel = kernel;
        }

        public string Suggest(Webpage parent,SuggestParams suggestParams)
        {
            IWebpageUrlGenerator generator = GetGenerator(suggestParams.DocumentType, suggestParams.Template);

            string url = generator.GetUrl(suggestParams.PageName, parent, suggestParams.UseHierarchy);

            //make sure the URL is unique

            if (!_urlValidationService.UrlIsValidForWebpage(url, null))
            {
                int counter = 1;

                while (!_urlValidationService.UrlIsValidForWebpage(string.Format("{0}-{1}", url, counter), null))
                    counter++;

                url = string.Format("{0}-{1}", url, counter);
            }
            return url;
        }

        private IWebpageUrlGenerator GetGenerator(string documentType, int? template)
        {
            IWebpageUrlGenerator generator = null;
            int id = template.GetValueOrDefault(0);
            if (id > 0)
            {
                var pageTemplate = _pageTemplateRepository.Get(id);
                Type urlGeneratorType = pageTemplate.GetUrlGeneratorType();
                if (pageTemplate != null && urlGeneratorType != null)
                {
                    generator = _kernel.Get(urlGeneratorType) as IWebpageUrlGenerator;
                }
            }
            if (generator == null && documentType != null)
            {
                generator = _kernel.Get(_settings.GetGeneratorType(documentType)) as IWebpageUrlGenerator;
            }
            return generator ?? new DefaultWebpageUrlGenerator();
        }
    }
}