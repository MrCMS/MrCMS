using System.ComponentModel;
using MrCMS.Models;
using MrCMS.Web.Admin.Infrastructure.Models;

namespace MrCMS.Web.Admin.Models
{
    public class MoveWebpageModel : IHaveId
    {
        int? IHaveId.Id => Id;
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [DisplayName("Update Urls?")]
        public bool UpdateUrls { get; set; }
    }
}