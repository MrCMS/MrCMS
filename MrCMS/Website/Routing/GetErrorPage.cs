using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Routing
{
    public class GetErrorPage : IGetErrorPage
    {
        private readonly IDocumentService _documentService;
        private readonly SiteSettings _siteSettings;

        public GetErrorPage(IDocumentService documentService, SiteSettings siteSettings)
        {
            _documentService = documentService;
            _siteSettings = siteSettings;
        }

        public Webpage GetPage(int code)
        {
            switch (code)
            {
                case 404:
                    return _documentService.GetDocument<Webpage>(_siteSettings.Error404PageId);
                case 401:
                case 403:
                    return _documentService.GetDocument<Webpage>(_siteSettings.Error403PageId);
                case 500:
                    return _documentService.GetDocument<Webpage>(_siteSettings.Error500PageId);
                default:
                    return null;
            }
        }
    }
}