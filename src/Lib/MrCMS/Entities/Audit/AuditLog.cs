using MrCMS.Entities.People;

namespace MrCMS.Entities.Audit
{
    public abstract class AuditLog : SystemEntity
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}