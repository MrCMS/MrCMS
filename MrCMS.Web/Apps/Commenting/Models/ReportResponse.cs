namespace MrCMS.Web.Apps.Commenting.Models
{
    public class ReportResponse : ICommentResponseInfo
    {
        public CommentResponseType Type { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}