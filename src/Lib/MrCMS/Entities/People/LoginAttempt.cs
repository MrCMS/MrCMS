namespace MrCMS.Entities.People
{
    public class LoginAttempt : SystemEntity
    {
        public virtual string Email { get; set; }
        public virtual User User { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual string UserAgent { get; set; }

        public virtual LoginAttemptStatus Status { get; set; }
    }
}