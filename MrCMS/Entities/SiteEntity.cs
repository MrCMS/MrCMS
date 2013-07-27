using System;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Entities
{
    public abstract class SystemEntity
    {
        public virtual int Id { get; set; }
        [DisplayName("Created On")]
        public virtual DateTime CreatedOn { get; set; }
        [DisplayName("Updated On")]
        public virtual DateTime UpdatedOn { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual void OnDeleting(ISession session)
        {
        }
    }

    public abstract class SiteEntity : SystemEntity
    {
        public virtual void CustomBinding(ControllerContext controllerContext, ISession session) { }

        public virtual Site Site { get; set; }
    }
}