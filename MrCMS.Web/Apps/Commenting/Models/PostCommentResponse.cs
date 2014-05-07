namespace MrCMS.Web.Apps.Commenting.Models
{
    public class PostCommentResponse : ICommentResponseInfo
    {
        public bool Valid { get; set; }
        public bool Pending { get; set; }

        public CommentResponseType Type
        {
            get
            {
                return !Valid
                    ? CommentResponseType.Error
                    : Pending
                        ? CommentResponseType.Info
                        : CommentResponseType.Success;
            }
        }

        public string Message { get; set; }
        public string RedirectUrl { get; set; }
    }
}