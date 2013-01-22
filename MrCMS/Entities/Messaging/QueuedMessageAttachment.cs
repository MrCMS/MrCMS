namespace MrCMS.Entities.Messaging
{
    public class QueuedMessageAttachment : SiteEntity
    {
        public virtual QueuedMessage QueuedMessage { get; set; }

        public virtual string FileName { get; set; }
    }
}