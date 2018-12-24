using System.ComponentModel;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public interface IHavePlaceholder
    {
        string PlaceHolder { get; set; }
    }

    public class TextBox : FormProperty, IHavePlaceholder
    {
        [DisplayName("Placeholder Text")]
        public virtual string PlaceHolder { get; set; }
    }
}