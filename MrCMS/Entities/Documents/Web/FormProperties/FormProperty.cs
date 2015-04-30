using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public abstract class FormProperty : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }

        [DisplayName("Label Text")]
        public virtual string LabelText { get; set; }

        [DisplayName("Field is Required")]
        public virtual bool Required { get; set; }

        [DisplayName("CSS Class")]
        public virtual string CssClass { get; set; }

        [DisplayName("HTML Id")]
        public virtual string HtmlId { get; set; }

        public virtual Webpage Webpage { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual string GetHtmlId()
        {
            string s = string.IsNullOrWhiteSpace(HtmlId) ? Name : HtmlId;
            return TagBuilder.CreateSanitizedId(s);
        }
    }
}