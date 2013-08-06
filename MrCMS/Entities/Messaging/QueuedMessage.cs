using System;
using System.Collections.Generic;

namespace MrCMS.Entities.Messaging
{
    public class QueuedMessage : SiteEntity
    {
        public virtual string FromAddress { get; set; }
        public virtual string FromName { get; set; }

        public virtual string ToAddress { get; set; }
        public virtual string ToName { get; set; }

        public virtual string Cc { get; set; }
        public virtual string Bcc { get; set; }

        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public virtual DateTime? SentOn { get; set; }

        public virtual int Tries { get; set; }

        public virtual bool FailedToSend { get { return Tries >= 5; } }

        public virtual bool IsHtml { get; set; }

        public virtual IList<QueuedMessageAttachment> QueuedMessageAttachments { get; set; }
    }
}
