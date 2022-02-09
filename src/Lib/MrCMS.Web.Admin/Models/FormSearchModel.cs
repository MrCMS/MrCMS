using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class FormSearchModel
    {
        [Display(Name = "Name or ID")]
        public string Name { get; set; }
        public int Page { get; set; } = 1;
    }
}