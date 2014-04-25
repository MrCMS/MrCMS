namespace MrCMS.Web.Apps.Commenting.Models
{
    public interface ICommentResponseInfo
    {
        CommentResponseType Type { get; }
        string Message { get; }
        string RedirectUrl { get; }
    }

    public static class CommentResponseInfoExtensions
    {
        public static bool IsSuccess(this ICommentResponseInfo commentResponseInfo)
        {
            return commentResponseInfo != null && commentResponseInfo.Type == CommentResponseType.Success;
        }
    }
}