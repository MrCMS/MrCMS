using System.Threading.Tasks;

namespace MrCMS.Services;

public class EnsureWebpageUrlIsValid : IEnsureWebpageUrlIsValid
{
    private readonly IUrlValidationService _urlValidationService;

    public EnsureWebpageUrlIsValid(IUrlValidationService urlValidationService)
    {
        _urlValidationService = urlValidationService;
    }

    public async Task<string> GetValidUrl(int siteId, string requestedUrl, int? existingPageId = null)
    {
        var url = requestedUrl;
        if (await _urlValidationService.UrlIsValidForWebpage(siteId, url, existingPageId)) return url;
        
        var counter = 1;

        while (!await _urlValidationService.UrlIsValidForWebpage(siteId, $"{url}-{counter}", existingPageId))
            counter++;

        url = $"{url}-{counter}";

        return url;
    }
}
