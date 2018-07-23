using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class TextBox : FormProperty
    {
        [DisplayName("Placeholder Text")]
        public virtual string PlaceHolder { get; set; }
    }
}