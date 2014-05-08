using MrCMS.Services;

namespace MrCMS.Tests.Services
{
    public class SiteMapServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly SiteMapService _siteMapService;

        public SiteMapServiceTests()
        {
            _siteMapService = new SiteMapService(Session, CurrentSite);
        }

    }
}