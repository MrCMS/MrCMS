using System.Collections.Generic;

namespace MrCMS.Entities.Documents
{
    public class Tag : SiteEntity
    {
        private IList<Document> _documents = new List<Document>();
        public virtual string Name { get; set; }

        public virtual IList<Document> Documents
        {
            get { return _documents; }
            protected internal set { _documents = value; }
        }
    }
}