using System.Collections;
using System.Web.Mvc;

namespace MrCMS.Entities.Documents.Web
{
    public class FormValue : SiteEntity
    {
        public virtual FormPosting FormPosting { get; set; }
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsFile { get; set; }

        public virtual string GetMessageValue()
        {
            return IsFile
                       ? string.Format("<a href=\"http://{0}{1}\">{1}</a>", Site.BaseUrl, Value)
                       : Value;
        }
    }
}