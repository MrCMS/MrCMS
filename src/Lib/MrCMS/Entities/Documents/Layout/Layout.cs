using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Layout
{
    public class Layout : Document
    {
        public Layout()
        {
            LayoutAreas = new List<LayoutArea>();
        }

        public override string UrlSegment { get; set; }

        public virtual IList<LayoutArea> LayoutAreas { get; set; }

        public virtual IList<PageTemplate> PageTemplates { get; set; }

        public virtual bool Hidden { get; set; }
    }
}