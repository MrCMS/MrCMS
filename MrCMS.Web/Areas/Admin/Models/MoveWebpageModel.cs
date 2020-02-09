using System.ComponentModel;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class MoveWebpageModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [DisplayName("Update Urls?")]
        public bool UpdateUrls { get; set; }
    }
}