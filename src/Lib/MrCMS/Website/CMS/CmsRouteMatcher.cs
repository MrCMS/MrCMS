using System;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class CmsRouteMatcher : ICmsRouteMatcher
    {
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ICanPreviewWebpage _canPreview;
        private readonly IDocumentMetadataService _documentMetadataService;
        private readonly IUserUIPermissionsService _userUIPermissionsService;

        public CmsRouteMatcher(IUserUIPermissionsService userUIPermissionsService, IGetWebpageForPath getWebpageForPath,
            ICanPreviewWebpage canPreview, IDocumentMetadataService documentMetadataService)
        {
            _userUIPermissionsService = userUIPermissionsService;
            _getWebpageForPath = getWebpageForPath;
            _canPreview = canPreview;
            _documentMetadataService = documentMetadataService;
        }

        public async Task<CmsMatchData> TryMatch(string path, string method)
        {
            var webpage = await _getWebpageForPath.GetWebpage(path);
            return await HandlePageData(webpage, method);
        }

        private async Task<CmsMatchData> HandlePageData(Webpage webpage, string method)
        {
            if (webpage == null)
            {
                return new CmsMatchData {MatchType = CmsRouteMatchType.NoMatch};
            }

            var isCurrentUserAllowed = await _userUIPermissionsService.IsCurrentUserAllowed(webpage);


            var matchType = isCurrentUserAllowed switch
            {
                PageAccessPermission.Forbidden => CmsRouteMatchType.Forbidden,
                PageAccessPermission.Unauthorized => CmsRouteMatchType.Unauthorised,
                _ => (CmsRouteMatchType?) null
            };
            PageData pageData = await GetData(webpage, method, isCurrentUserAllowed);
            // if we've not already worked it out, use the page data to see what we're going to do with the match type
            matchType ??= pageData.DisplayState switch
            {
                PageDataState.Display => CmsRouteMatchType.Success,
                PageDataState.Preview => CmsRouteMatchType.Preview,
                PageDataState.Unpublished => CmsRouteMatchType.NoMatch,
                _ => throw new ArgumentOutOfRangeException(nameof(pageData.DisplayState))
            };

            return new CmsMatchData
            {
                MatchType = matchType!.Value,
                PageData = pageData
            };
        }

        public async Task<CmsMatchData> Match(Webpage webpage, string method)
        {
            return await HandlePageData(webpage, method);
        }

        private async Task<PageData> GetData(Webpage webpage, string method, PageAccessPermission permission)
        {
            if (webpage == null)
            {
                return null;
            }

            var unpublished = !webpage.Published;
            if (unpublished && !await _canPreview.CanPreview(webpage))
            {
                return new PageData
                {
                    DisplayState = PageDataState.Unpublished,
                    Id = webpage.Id,
                    Type = webpage.GetType(),
                    Controller = "Error",
                    Action = "Handle404",
                    Webpage = webpage
                };
            }

            var metadata = _documentMetadataService.GetMetadata(webpage);

            return new PageData
            {
                DisplayState = unpublished ? PageDataState.Preview : PageDataState.Display,
                Id = webpage.Id,
                Type = webpage.GetType(),
                Controller = metadata.GetController(method, permission),
                Action = metadata.GetAction(method, permission),
                Webpage = webpage
            };
        }
    }
}