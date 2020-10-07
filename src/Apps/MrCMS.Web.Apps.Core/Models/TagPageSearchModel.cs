namespace MrCMS.Web.Apps.Core.Models
{
    public class TagPageSearchModel
    {
        public TagPageSearchModel()
        {
            Page = 1;
            PageSize = 20;
        }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}