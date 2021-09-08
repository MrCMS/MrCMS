namespace MrCMS.Web.Admin.Models
{
    public class PushNotificationSearchModel 
    {
        public PushNotificationSearchModel()
        {
            P = 1;
        }

        public int P { get; set; }
        public int PageNumber => P;
    }
}