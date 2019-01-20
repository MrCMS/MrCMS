using System.ComponentModel;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MergeWebpageModel
    {
        public int Id { get; set; }
        public int MergeIntoId { get; set; }
        [DisplayName("Update Urls?")]
        public bool UpdateUrls { get; set; }
    }
}