using System;
using System.ComponentModel;

namespace MrCMS.Entities
{
    public abstract class SystemEntity : IHaveId, IHaveSystemDates, ICanSoftDelete
    {
        protected SystemEntity()
        {
            Guid = Guid.NewGuid();
        }

        public virtual int Id { get; set; }

        //[DbNotNullable, ShouldMap]
        public virtual Guid Guid { get; set; }

        [DisplayName("Created On")]
        public virtual DateTime CreatedOn { get; set; }

        [DisplayName("Updated On")]
        public virtual DateTime UpdatedOn { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}