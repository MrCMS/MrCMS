using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class Email : FormProperty, IHavePlaceholder
    {
        [DisplayName("Placeholder Text")]
        public virtual string PlaceHolder { get; set; }
    }
}