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
            _webpageVersionRepository = new InMemoryRepository<WebpageVersion>();
            _inMemoryRepository = new InMemoryRepository<Webpage>();
            _webpageVersionsAdminService = new WebpageVersionsAdminService(_webpageVersionRepository,
                _inMemoryRepository);
        }

        private readonly WebpageVersionsAdminService _webpageVersionsAdminService;
        private readonly InMemoryRepository<WebpageVersion> _webpageVersionRepository;
        private readonly InMemoryRepository<Webpage> _inMemoryRepository;

        [Fact]
        public async Task WebpageVersionService_GetDocumentVersion_GetsTheVersionWithTheRequestedId()
        {
            var documentVersion = new WebpageVersion();
            await _webpageVersionRepository.Add(documentVersion);

            var version = _webpageVersionsAdminService.GetWebpageVersion(documentVersion.Id);

            version.Should().Be(documentVersion);
        }
    }
}