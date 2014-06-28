using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Services
{
    public class WebpageUrlService : IWebpageUrlService
    {
        private readonly IUrlValidationService _urlValidationService;
        private readonly IKernel _kernel;
        private static readonly Dictionary<string, Type> _webpageGenerators;

        public WebpageUrlService(IUrlValidationService urlValidationService, IKernel kernel)
        {
            _urlValidationService = urlValidationService;
            _kernel = kernel;
        }

        static WebpageUrlService()
        {
            _webpageGenerators = new Dictionary<string, Type>();

            foreach (var type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>().Where(type => !type.ContainsGenericParameters))
            {
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(WebpageUrlGenerator<>).MakeGenericType(type));
                if (types.Any())
                {
                    _webpageGenerators.Add(type.FullName, types.First());
                }
            }
        }

        public static Dictionary<string, Type> WebpageGenerators
        {
            get { return _webpageGenerators; }
        }

        public string Suggest(string pageName, Webpage parent, string documentType, bool useHierarchy = false)
        {
            var generator = GetGenerator(documentType);

            var url = generator.GetUrl(pageName, parent, useHierarchy);

            //make sure the URL is unique

            if (!_urlValidationService.UrlIsValidForWebpage(url, null))
            {
                var counter = 1;

                while (!_urlValidationService.UrlIsValidForWebpage(string.Format("{0}-{1}", url, counter), null))
                    counter++;

                url = string.Format("{0}-{1}", url, counter);
            }
            return url;
        }

        private IWebpageUrlGenerator GetGenerator(string documentType)
        {
            IWebpageUrlGenerator generator = null;
            if (_webpageGenerators.ContainsKey(documentType))
            {
                generator = _kernel.Get(_webpageGenerators[documentType]) as IWebpageUrlGenerator;
            }
            if (generator == null)
            {
                generator = new DefaultWebpageUrlGenerator();
            }
            return generator;
        }
    }
}