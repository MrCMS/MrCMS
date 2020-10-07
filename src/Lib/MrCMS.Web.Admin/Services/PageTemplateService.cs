using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Web.Admin.Services
{
    public class PageTemplateService : IPageTemplateService
    {
        private readonly IDocumentMetadataService _documentMetadataService;

        public PageTemplateService(IDocumentMetadataService documentMetadataService)
        {
            _documentMetadataService = documentMetadataService;
        }

        private Type GetPageType(PageTemplate template)
        {
            return template != null
                ? TypeHelper.GetTypeByName(template.PageType)
                : null;
        }
        public string GetLayoutName(PageTemplate template)
        {
            return template != null && template.Layout != null
                ? template.Layout.Name
                : string.Empty;
        }
        public string GetPageTypeName(PageTemplate template)
        {
            var pageType = GetPageType(template);
            if (pageType == null) return string.Empty;

            var metadata = _documentMetadataService.GetMetadata(pageType);

            return metadata.Name;
        }

        public Type GetUrlGeneratorType(PageTemplate template)
        {
            return template != null
                ? TypeHelper.GetTypeByName(template.UrlGeneratorType)
                : null;
        }
    }
}