using System;
using System.ComponentModel;
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
}