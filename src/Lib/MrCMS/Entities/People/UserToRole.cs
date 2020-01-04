namespace MrCMS.Entities.People
{
    public class UserToRole
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public UserRole Role { get; set; }
        public int RoleId { get; set; }
    }
}