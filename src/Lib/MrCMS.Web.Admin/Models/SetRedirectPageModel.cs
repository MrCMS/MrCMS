using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class SetRedirectPageModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Page")]
        public int PageId { get; set; }
    }
}