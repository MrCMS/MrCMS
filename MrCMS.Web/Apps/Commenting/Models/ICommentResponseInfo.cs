namespace MrCMS.Web.Apps.Commenting.Models
{
    public interface ICommentResponseInfo
    {
        CommentResponseType Type { get; }
        string Message { get; }
        string RedirectUrl { get; }
    }
}