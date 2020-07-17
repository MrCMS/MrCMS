using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class AddFormPropertyModel
    {
        public int FormId { get; set; }

        public string PropertyType { get; set; }

        [DisplayName("Field is Required")]
        public bool Required { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string LabelText { get; set; }

        [DisplayName("CSS Class")]
        [MaxLength(200)]
        public string CssClass { get; set; }

        [DisplayName("HTML Id")]
        [MaxLength(50)]
        public string HtmlId { get; set; }
    }
}