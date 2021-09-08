namespace MrCMS.Web.Admin.Models.Forms
{
    public class WebpagesWithEmbeddedFormQuery
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public WebpagesWithEmbeddedFormQuery()
        {
            Page = 1;
            PageSize = 30;
        }
    }
}