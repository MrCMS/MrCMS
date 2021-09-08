namespace MrCMS.Website.CMS
{
    public class CmsMatchData
    {
        public CmsRouteMatchType MatchType { get; set; }
        public PageData PageData { get; set; }

        public bool WillRender => MatchType == CmsRouteMatchType.Success || MatchType == CmsRouteMatchType.Preview;
    }
}