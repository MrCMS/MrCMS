using System;
using System.ComponentModel;
using MrCMS.DbConfiguration;

namespace MrCMS.Entities
{
    public abstract class SystemEntity
    {
        private Guid _guid;

        protected SystemEntity()
        {
            _guid = Guid.NewGuid();
        }
        public virtual int Id { get; set; }

        [NotNullable, ShouldMapAttribute]
        public virtual Guid Guid { get { return _guid; } }

        [DisplayName("Created On")]
        public virtual DateTime CreatedOn { get; set; }

        [DisplayName("Updated On")]
        public virtual DateTime UpdatedOn { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}