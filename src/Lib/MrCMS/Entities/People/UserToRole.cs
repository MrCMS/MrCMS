using MrCMS.Data;

namespace MrCMS.Entities.People
{
    public class UserToRole : IJoinTable
    {
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual Role UserRole { get; set; }
        public int UserRoleId { get; set; }
    }
}