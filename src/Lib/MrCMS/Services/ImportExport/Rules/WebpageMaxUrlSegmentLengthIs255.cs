namespace MrCMS.Services.ImportExport.Rules
{
    public class WebpageMaxUrlSegmentLengthIs255 : WebpageMaxStringLength
    {
        public WebpageMaxUrlSegmentLengthIs255()
            : base("UrlSegment", o => o.UrlSegment, 255)
        {
        }
    }
}