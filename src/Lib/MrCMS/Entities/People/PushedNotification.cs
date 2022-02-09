namespace MrCMS.Entities.People
{
    public class PushNotification : SiteEntity
    {
        public virtual string Title { get; set; }

        public virtual string Body { get; set; }

        public virtual string Icon { get; set; }

        public virtual string Badge { get; set; }

        public virtual string ActionUrl { get; set; }
        public virtual string Image { get; set; }
    }
}