namespace MrCMS.Web.Apps.Commenting.Models
{
    public class VoteModel : IHaveIPAddress
    {
        public int CommentId { get; set; }
        public string IPAddress { get; set; }
    }
}