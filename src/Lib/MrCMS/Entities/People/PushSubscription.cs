namespace MrCMS.Entities.People
{
    public class PushSubscription : SiteEntity
    {
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual string Endpoint { get; set; }
        public virtual string Key { get; set; }
        public virtual string AuthSecret { get; set; }
    }
}