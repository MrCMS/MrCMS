using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public abstract class FormProperty : SiteEntity
    {
        protected FormProperty()
        {
            Options = new List<FormListOption>();
        }
        public virtual string Name { get; set; }
        public virtual string LabelText { get; set; }
        public virtual bool Required { get; set; }
        public virtual string CssClass { get; set; }
        public virtual string HtmlId { get; set; }

        public virtual Webpage Webpage { get; set; }
        public virtual IList<FormListOption> Options { get; set; }
        public abstract bool HasOptions { get; }
        public virtual int DisplayOrder { get; set; }

        public virtual string GetHtmlId()
        {
            var s = string.IsNullOrWhiteSpace(HtmlId) ? Name : HtmlId;
            return TagBuilder.CreateSanitizedId(s);
        }
    }
}