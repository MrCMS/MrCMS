namespace MrCMS.Entities.Messaging
{
    public class QueuedMessageAttachment : SiteEntity
    {
        public virtual QueuedMessage QueuedMessage { get; set; }

        public virtual byte[] Data { get; set; }
        public virtual string ContentType { get; set; }
        public virtual long FileSize { get; set; }
        public virtual string FileName { get; set; }
    }
}