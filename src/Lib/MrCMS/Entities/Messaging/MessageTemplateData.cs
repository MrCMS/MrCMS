namespace MrCMS.Entities.Messaging
{
    public class MessageTemplateData : SystemEntity
    {
        public virtual string Type { get; set; }
        public virtual string Data { get; set; }
        public virtual int? SiteId { get; set; }
    }
}