using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class WebpageVersionsAdminServiceTests
    {
        public WebpageVersionsAdminServiceTests()
        {
            _documentVersionRepository = new InMemoryRepository<WebpageVersion>();
            _inMemoryRepository = new InMemoryRepository<Webpage>();
            _webpageVersionsAdminService = new WebpageVersionsAdminService(_documentVersionRepository,
                _inMemoryRepository);
        }

        private readonly WebpageVersionsAdminService _webpageVersionsAdminService;
        private readonly InMemoryRepository<WebpageVersion> _documentVersionRepository;
        private readonly InMemoryRepository<Webpage> _inMemoryRepository;

        [Fact]
        public async Task WebpageVersionService_GetDocumentVersion_GetsTheVersionWithTheRequestedId()
        {
            var documentVersion = new WebpageVersion();
            await _documentVersionRepository.Add(documentVersion);

            var version = _webpageVersionsAdminService.GetDocumentVersion(documentVersion.Id);

            version.Should().Be(documentVersion);
        }
    }
}