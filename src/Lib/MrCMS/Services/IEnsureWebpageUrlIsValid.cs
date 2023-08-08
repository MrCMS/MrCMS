using System.Threading.Tasks;

namespace MrCMS.Services;

public interface IEnsureWebpageUrlIsValid
{
    Task<string> GetValidUrl(int siteId, string requestedUrl, int? existingPageId = null);
}