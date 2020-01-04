namespace MrCMS.Entities.People
{
    public class LoginAttempt : SiteEntity
    {
        public virtual string Email { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual string UserAgent { get; set; }

        public virtual LoginAttemptStatus Status { get; set; }
    }
}