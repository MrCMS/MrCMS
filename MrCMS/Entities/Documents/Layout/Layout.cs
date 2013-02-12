using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Entities.Documents.Layout
{
    //[DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Layout", IconClass = "icon-th", DisplayOrder = 1, Type = typeof(Layout), WebGetAction = "View", WebGetController = "Layout")]
    public class Layout : Document
    {
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
