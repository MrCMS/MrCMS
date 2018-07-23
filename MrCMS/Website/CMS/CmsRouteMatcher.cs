using System.Threading.Tasks;

namespace MrCMS.Website.CMS
{
    public class CmsRouteMatcher : ICmsRouteMatcher
    {
        private readonly IGetPageData _getPageData;

        public CmsRouteMatcher(IGetPageData getPageData)
        {
            _getPageData = getPageData;
        }

        public async Task<CmsMatchData> TryMatch(string path)
        {
            var pageData = await _getPageData.GetData(path);
            if (pageData == null)
                return new CmsMatchData { MatchType = CmsRouteMatchType.NoMatch };
            return new CmsMatchData
            {
                MatchType = pageData.IsPreview ? CmsRouteMatchType.Preview : CmsRouteMatchType.Success,
                PageData = pageData
            };
        }
    }
}