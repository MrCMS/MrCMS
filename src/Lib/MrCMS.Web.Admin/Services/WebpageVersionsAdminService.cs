using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class WebpageVersionsAdminService : IWebpageVersionsAdminService
    {
        private readonly IRepository<WebpageVersion> _webpageVersionRepository;
        private readonly IRepository<Webpage> _webpageRepository;

        public WebpageVersionsAdminService(IRepository<WebpageVersion> webpageVersionRepository,
            IRepository<Webpage> webpageRepository)
        {
            _webpageVersionRepository = webpageVersionRepository;
            _webpageRepository = webpageRepository;
        }

        public async Task<VersionsModel> GetVersions(Webpage webpage, int page)
        {
            IPagedList<WebpageVersionViewModel> versions = await _webpageVersionRepository.Query()
                .Where(version => version.Webpage.Id == webpage.Id)
                .OrderByDescending(version => version.Id)//id faster than createdon for ordering
                .Select(item => new WebpageVersionViewModel
                {
                    Id = item.Id,
                    CreatedOn = item.CreatedOn,
                    FirstName = item.User == null ? "System" : string.IsNullOrWhiteSpace(item.User.FirstName + item.User.LastName) ? item.User.Email : item.User.FirstName,
                    LastName = string.IsNullOrWhiteSpace(item.User.FirstName + item.User.LastName) ? "" : item.User.LastName
                })
                .PagedAsync(page);

            return new VersionsModel(versions, webpage.Id);
        }

        public async Task<WebpageVersion> GetWebpageVersion(int id)
        {
            return await _webpageVersionRepository.Get(id);
        }

        public async Task<WebpageVersion> RevertToVersion(int id)
        {
            var documentVersion = await GetWebpageVersion(id);

            var currentVersion = documentVersion.Webpage.Unproxy();
            var previousVersion = currentVersion.GetVersion(documentVersion.Id);

            var versionProperties = currentVersion.GetType().GetVersionProperties();
            foreach (var versionProperty in versionProperties)
            {
                versionProperty.SetValue(currentVersion, versionProperty.GetValue(previousVersion, null), null);
            }

            await _webpageRepository.Update(currentVersion);
            return documentVersion;
        }
    }
}