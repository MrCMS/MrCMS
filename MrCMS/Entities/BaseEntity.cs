using System;
using System.ComponentModel;
using System.Web.Mvc;
using NHibernate;

namespace MrCMS.Entities
{
    public abstract class BaseEntity 
    {
        public virtual int Id { get; set; }
        [DisplayName("Created On")]
        public virtual DateTime CreatedOn { get; set; }
        [DisplayName("Updated On")]
        public virtual DateTime UpdatedOn { get; set; }

        public virtual void OnDeleting()
        {
        }

        public virtual void CustomBinding(ControllerContext controllerContext, ISession session) { }
    }
}