using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class DocumentVersionsAdminService : IDocumentVersionsAdminService
    {
        private readonly IRepository<DocumentVersion> _documentVersionRepository;
        private readonly IRepository<Document> _documentRepository;
        private readonly ILogger<IDocumentVersionsAdminService> _logger;

        public DocumentVersionsAdminService(IRepository<DocumentVersion> documentVersionRepository,
            IRepository<Document> documentRepository, ILogger<IDocumentVersionsAdminService> logger)
        {
            _documentVersionRepository = documentVersionRepository;
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<VersionsModel> GetVersions(Document document, int page)
        {
            _logger.Log(LogLevel.Information, message:page.ToString());
            IPagedList<DocumentVersionViewModel> versions = await _documentVersionRepository.Query()
                .Where(version => version.Document.Id == document.Id)
                .OrderByDescending(version => version.Id)//id faster than createdon for ordering
                .Select(item => new DocumentVersionViewModel
                {
                    Id = item.Id,
                    CreatedOn = item.CreatedOn,
                    FirstName = item.User.FirstName,
                    LastName = item.User.LastName
                })
                .PagedAsync(page);

            return new VersionsModel(versions, document.Id);
        }

        public async Task<DocumentVersion> GetDocumentVersion(int id)
        {
            return await _documentVersionRepository.Get(id);
        }

        public async Task<DocumentVersion> RevertToVersion(int id)
        {
            var documentVersion = await GetDocumentVersion(id);

            var currentVersion = documentVersion.Document.Unproxy();
            var previousVersion = currentVersion.GetVersion(documentVersion.Id);

            var versionProperties = currentVersion.GetType().GetVersionProperties();
            foreach (var versionProperty in versionProperties)
            {
                versionProperty.SetValue(currentVersion, versionProperty.GetValue(previousVersion, null), null);
            }

            await _documentRepository.Update(currentVersion);
            return documentVersion;
        }
    }
}