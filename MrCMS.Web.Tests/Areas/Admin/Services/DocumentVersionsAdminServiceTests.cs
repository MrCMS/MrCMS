using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class DocumentVersionsAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly DocumentVersionsAdminService _documentVersionsAdminService;

        public DocumentVersionsAdminServiceTests()
        {
            _documentVersionsAdminService = new DocumentVersionsAdminService(Session);
        }

        [Fact]
        public void DocumentVersionService_GetDocumentVersion_GetsTheVersionWithTheRequestedId()
        {
            var documentVersion = new DocumentVersion();
            Session.Transact(session => session.Save(documentVersion));

            DocumentVersion version = _documentVersionsAdminService.GetDocumentVersion(documentVersion.Id);

            version.Should().Be(documentVersion);
        }
    }
}