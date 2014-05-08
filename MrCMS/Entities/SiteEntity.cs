using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;
using Ninject;

namespace MrCMS.Entities
{
    public abstract class SiteEntity : SystemEntity
    {
        public virtual void CustomBinding(ControllerContext controllerContext, IKernel kernel) { }

        public virtual Site Site { get; set; }

        public virtual string SiteName { get { return Site != null ? Site.DisplayName : null; } }
    }
}