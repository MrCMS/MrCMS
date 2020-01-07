using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class MoveWebpageModel : IHaveId
    {
        int IHaveId.Id => Id;
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [DisplayName("Update Urls?")]
        public bool UpdateUrls { get; set; }
    }
}