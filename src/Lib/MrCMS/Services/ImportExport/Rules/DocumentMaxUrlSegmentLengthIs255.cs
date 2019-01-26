namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentMaxUrlSegmentLengthIs255 : DocumentMaxStringLength
    {
        public DocumentMaxUrlSegmentLengthIs255()
            : base("UrlSegment", o => o.UrlSegment, 255)
        {
        }
    }
}