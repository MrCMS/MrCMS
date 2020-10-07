using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class CmsRouteMatcher : ICmsRouteMatcher
    {
        private readonly IGetPageData _getPageData;
        private readonly IUserUIPermissionsService _userUIPermissionsService;

        public CmsRouteMatcher(IGetPageData getPageData, IUserUIPermissionsService userUIPermissionsService)
        {
            _getPageData = getPageData;
            _userUIPermissionsService = userUIPermissionsService;
        }

        public CmsMatchData TryMatch(string path, string method)
        {
            var pageData = _getPageData.GetData(path, method);
            return HandlePageData(pageData);
        }

        private CmsMatchData HandlePageData(PageData pageData)
        {
            if (pageData == null)
            {
                return new CmsMatchData {MatchType = CmsRouteMatchType.NoMatch};
            }

            var isCurrentUserAllowed = _userUIPermissionsService.IsCurrentUserAllowed(pageData.Webpage);

            return new CmsMatchData
            {
                MatchType = !isCurrentUserAllowed
                    ? CmsRouteMatchType.Disallowed
                    : pageData.IsPreview
                        ? CmsRouteMatchType.Preview
                        : CmsRouteMatchType.Success,
                PageData = pageData
            };
        }

        public CmsMatchData Match(Webpage webpage, string method)
        {
            var pageData = _getPageData.GetData(webpage, method);
            return HandlePageData(pageData);
        }
    }
}