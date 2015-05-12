namespace MrCMS.Web.Areas.Admin.Models
{
    public class WebpageSearchQuery
    {
        public WebpageSearchQuery()
        {
            Page = 1;
        }

        public int Page { get; set; }

        public int ParentId { get; set; }

        public string Query { get; set; }
    }
}