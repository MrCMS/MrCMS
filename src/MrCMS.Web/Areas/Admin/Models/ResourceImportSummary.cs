namespace MrCMS.Web.Areas.Admin.Models
{
    public class ResourceImportSummary
    {
        public int Added { get; set; }
        public int Updated { get; set; }
        public int Processed { get { return Added + Updated; } }
    }
}