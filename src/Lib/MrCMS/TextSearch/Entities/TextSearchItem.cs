using System;
using System.ComponentModel.DataAnnotations;
using MrCMS.DbConfiguration;

namespace MrCMS.TextSearch.Entities
{
    [ShouldMapEntity]
    public class TextSearchItem
    {
        public virtual int Id { get; set; }

        internal const int MaxTextLength = 400;
        internal const int MaxDisplayNameLength = 100;
        [Required, MaxLength(MaxTextLength)] public virtual string Text { get; set; }

        [Required, MaxLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required, MaxLength(255)] public virtual string SystemType { get; set; }
        [MaxLength(50)] public virtual string EntityType { get; set; }
        public virtual int EntityId { get; set; }
        public virtual int? SiteId { get; set; }
        public virtual DateTime EntityCreatedOn { get; set; }
        public virtual DateTime EntityUpdatedOn { get; set; }
    }
}