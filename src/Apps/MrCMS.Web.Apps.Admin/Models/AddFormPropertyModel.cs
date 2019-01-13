using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddFormPropertyModel
    {
        public int FormId { get; set; }

        public string PropertyType { get; set; }

        [DisplayName("Field is Required")]
        public bool Required { get; set; }

        [Required]
        public string Name { get; set; }

        public string LabelText { get; set; }

        [DisplayName("CSS Class")]
        public string CssClass { get; set; }

        [DisplayName("HTML Id")]
        public string HtmlId { get; set; }
    }
}