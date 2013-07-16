namespace MrCMS.Entities.Messaging
{
    public abstract class MessageTemplate : SiteEntity
    {
        public virtual string FromAddress { get; set; }
        public virtual string FromName { get; set; }

        public virtual string ToAddress { get; set; }
        public virtual string ToName { get; set; }

        public virtual string Cc { get; set; }
        public virtual string Bcc { get; set; }

        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public virtual bool IsHtml { get; set; }

        public abstract MessageTemplate GetInitialTemplate();
    }
}
