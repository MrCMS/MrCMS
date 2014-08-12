using System;
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
        private readonly IKernel _kernel;
        private readonly ISession _session;
        private readonly PageDefaultsSettings _settings;
        private readonly IUrlValidationService _urlValidationService;

        public WebpageUrlService(IUrlValidationService urlValidationService, ISession session, IKernel kernel,
            PageDefaultsSettings settings)
        {
            _urlValidationService = urlValidationService;
            _session = session;
            _kernel = kernel;
            _settings = settings;
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
                var pageTemplate = _session.Get<PageTemplate>(id);
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