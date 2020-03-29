using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddFormModel
    {
        [Required]
        public string Name { get; set; }
    }
}