using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Entities.Documents.Layout
{
    public class Layout : Document
    {
        public Layout()
        {
            LayoutAreas = new List<LayoutArea>();
        }
        [Required]
        [Remote("ValidateUrlIsAllowed", "Layout", AdditionalFields = "Id")]
        [DisplayName("Path")]
        public override string UrlSegment { get; set; }

        //todo this can't be protected, required in admin on layoput edit
        public virtual IList<LayoutArea> LayoutAreas { get; set; }

        public virtual IEnumerable<LayoutArea> GetLayoutAreas()
        {
            var layout = this;
            var layoutAreas = new List<LayoutArea>();
            while (layout != null)
            {
                layoutAreas.AddRange(layout.LayoutAreas);
                layout = layout.Parent.Unproxy() as Layout;
            }

            return layoutAreas;
        }

        public virtual IList<Webpage> Webpages { get; set; }

        public virtual bool Hidden { get; set; }

        public override void OnDeleting(ISession session)
        {
            foreach (var webpage in Webpages)
            {
                webpage.Layout = null;
            }
            Webpages.Clear();
            base.OnDeleting(session);
        }
    }
}
