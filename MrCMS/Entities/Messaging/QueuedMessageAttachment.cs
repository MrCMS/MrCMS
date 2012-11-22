namespace MrCMS.Entities.Messaging
{
    public class QueuedMessageAttachment : BaseEntity
    {
        public virtual QueuedMessage QueuedMessage { get; set; }

        public virtual string FileName { get; set; }
    }
}