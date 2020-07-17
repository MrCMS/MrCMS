using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class UpdateFormPropertyModel
    {
        public int Id { get; set; }

        [DisplayName("Field is Required")]
        public bool Required { get; set; }

        [Required]
        public string Name { get; set; }

        public string LabelText { get; set; }

        [DisplayName("CSS Class")]
        public string CssClass { get; set; }

        [DisplayName("HTML Id")]
        public string HtmlId { get; set; }

        public bool ShowPlaceholder { get; set; }

        public string Placeholder { get; set; }
    }
}