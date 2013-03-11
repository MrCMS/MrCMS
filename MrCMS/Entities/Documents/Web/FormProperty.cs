using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web
{
    public abstract class FormProperty : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual string LabelText { get; set; }
        public virtual bool Required { get; set; }
        public virtual string CssClass { get; set; }
        public virtual string HtmlId { get; set; }

        public virtual Webpage Webpage { get; set; }
        public virtual IList<FormListOption> Options { get; set; }
        public abstract bool HasOptions { get; }
    }

    public class TextBox : FormProperty
    {
        public override bool HasOptions { get { return false; } }
    }

    public class TextArea : FormProperty
    {
        public override bool HasOptions { get { return false; } }
    }


    public class DropDownList : FormProperty
    {
        public override bool HasOptions { get { return true; } }
    }

    public class CheckboxList : FormProperty
    {
        public override bool HasOptions { get { return true; } }
    }

    public class RadioButtonList : FormProperty
    {
        public override bool HasOptions { get { return true; } }
    }

    public class FileUpload : FormProperty
    {
        public override bool HasOptions { get { return false; } }
    }

    public class FormListOption : SiteEntity
    {
        public virtual FormProperty FormProperty { get; set; }
        public virtual string Value { get; set; }
        public virtual bool Selected { get; set; }
    }
}