using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public abstract class FormPropertyWithOptions : FormProperty
    {
        protected FormPropertyWithOptions()
        {
            Options = new List<FormListOption>();
        }
        public virtual IList<FormListOption> Options { get; set; }
        public abstract bool OnlyOneOptionSelectable { get; }
    }
}