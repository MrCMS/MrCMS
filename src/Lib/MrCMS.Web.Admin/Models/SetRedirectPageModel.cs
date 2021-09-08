using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class SetRedirectPageModel
    {
        public int Id { get; set; }

        [Required]
        public int PageId { get; set; }
    }
}