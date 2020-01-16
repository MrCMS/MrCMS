using MrCMS.Data;

namespace MrCMS.Entities.People
{
    public class UserToRole : IJoinTable
    {
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual UserRole Role { get; set; }
        public int RoleId { get; set; }
    }
}