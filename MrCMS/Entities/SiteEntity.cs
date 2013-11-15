using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Entities
{
    public abstract class SiteEntity : SystemEntity
    {
        public virtual void CustomBinding(ControllerContext controllerContext, ISession session) { }

        public virtual Site Site { get; set; }

        public virtual string SiteName { get { return Site != null ? Site.DisplayName : null; } }
    }
}