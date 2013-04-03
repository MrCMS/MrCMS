using System.Web.Mvc;
using MrCMS.Entities.Documents.Web.FormProperties;

namespace MrCMS.Shortcodes.Forms
{
    public interface IFormElementRenderer<T> : IFormElementRenderer where T : FormProperty 
    {
    }

    public interface IFormElementRenderer
    {
        TagBuilder AppendElement(FormProperty formProperty, string existingValue);
        bool IsSelfClosing { get; }
    }
}