namespace MrCMS.Web.Apps.Admin.Models
{
    public class UpdateLuceneFieldBoostModel
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Definition { get; set; }
        public float Boost { get; set; }
    }
}