using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Entities.Documents.Web
{
    public class FormPosting : SiteEntity
    {
        public FormPosting()
        {
            FormValues = new List<FormValue>();
        }
        public virtual Webpage Webpage { get; set; }
        public virtual IList<FormValue> FormValues { get; set; }

        public virtual string this[string heading]
        {
            get
            {
                return FormValues.Any(value => value.Key.Equals(heading, StringComparison.OrdinalIgnoreCase))
                           ? FormValues.First(value => value.Key.Equals(heading, StringComparison.OrdinalIgnoreCase)).
                                 Value
                           : string.Empty;
            }
        }
        public virtual FormValue Get(string heading)
        {
            return FormValues.FirstOrDefault(value => value.Key.Equals(heading, StringComparison.OrdinalIgnoreCase));
        }
    }
}