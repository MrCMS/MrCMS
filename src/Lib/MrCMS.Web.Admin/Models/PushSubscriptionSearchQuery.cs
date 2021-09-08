namespace MrCMS.Web.Admin.Models
{
    public class PushSubscriptionSearchQuery
    {
        public PushSubscriptionSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        
        public string Email { get; set; }
    }
}