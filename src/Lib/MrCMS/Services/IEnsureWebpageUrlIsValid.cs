using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services;

public interface IEnsureWebpageUrlIsValid
{
    Task<string> GetValidUrl(int siteId, string requestedUrl, int? existingPageId = null);
}