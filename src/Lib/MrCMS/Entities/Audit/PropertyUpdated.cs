using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Audit
{
    public class PropertyUpdated : IHaveId
    {
        public int Id { get; set; }

        public EntityUpdated EntityUpdated { get; set; }
        public int EntityUpdatedId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string OriginalValue { get; set; }
        public string CurrentValue { get; set; }
    }
}