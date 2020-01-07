using MrCMS.Data;

namespace MrCMS.Entities.People
{
    public class UserToRole : IJoinTable
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public UserRole Role { get; set; }
        public int RoleId { get; set; }
    }
}