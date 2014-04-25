using System;
using System.Linq;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.Web.Apps.Commenting.Entities
{
    public class Comment : SiteEntity
    {
        public Comment()
        {
            Children = new List<Comment>();
            Guid = Guid.NewGuid();
            Votes = new List<Vote>();
        }

        public virtual Webpage Webpage { get; set; }

        public virtual Guid Guid { get; set; }
        public virtual Comment InReplyTo { get; set; }
        public virtual IList<Comment> Children { get; set; }
        public virtual IList<Vote> Votes { get; set; }

        public virtual User User { get; set; }

        public virtual string Name { get; set; }
        public virtual string Email { get; set; }

        public virtual string IPAddress { get; set; }

        public virtual bool? Approved { get; set; }

        public virtual string Message { get; set; }

        public virtual string MessageTruncated(int maxSize = 200)
        {
            if (Message == null || Message.Length <= maxSize)
                return Message;
            int iNextSpace = Message.LastIndexOf(" ", maxSize, StringComparison.Ordinal);
            return string.Format("{0}...", Message.Substring(0, (iNextSpace > 0) ? iNextSpace : maxSize).Trim());
        }
    }
}