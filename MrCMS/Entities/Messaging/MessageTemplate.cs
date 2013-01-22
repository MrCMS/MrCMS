namespace MrCMS.Entities.Messaging
{
    public class MessageTemplate : SiteEntity
    {
        //public virtual MessageTemplateHeader Header { get; set; }
        public virtual string TemplateName { get; set; }

        public virtual string FromAddress { get; set; }
        public virtual string FromName { get; set; }

        public virtual string ToAddress { get; set; }
        public virtual string ToName { get; set; }

        public virtual string Cc { get; set; }
        public virtual string Bcc { get; set; }

        public virtual int Priority { get; set; }

        public virtual string Language { get; set; }

        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public virtual string FullQualifiedTypeName { get; set; }
    }
}
