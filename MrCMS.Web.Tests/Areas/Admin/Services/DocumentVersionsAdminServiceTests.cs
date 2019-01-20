using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class DocumentVersionsAdminServiceTests
    {
        public DocumentVersionsAdminServiceTests()
        {
            _documentVersionRepository = new InMemoryRepository<DocumentVersion>();
            _inMemoryRepository = new InMemoryRepository<Document>();
            _documentVersionsAdminService = new DocumentVersionsAdminService(_documentVersionRepository,
                _inMemoryRepository);
        }

        private readonly DocumentVersionsAdminService _documentVersionsAdminService;
        private readonly InMemoryRepository<DocumentVersion> _documentVersionRepository;
        private readonly InMemoryRepository<Document> _inMemoryRepository;

        [Fact]
        public void DocumentVersionService_GetDocumentVersion_GetsTheVersionWithTheRequestedId()
        {
            var documentVersion = new DocumentVersion();
            _documentVersionRepository.Add(documentVersion);

            var version = _documentVersionsAdminService.GetDocumentVersion(documentVersion.Id);

            version.Should().Be(documentVersion);
        }
    }
}