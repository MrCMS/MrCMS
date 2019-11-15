using System;
using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    

    public class PersistedGrant : SystemEntity
    {
       public virtual string Key { get; set; }

        public virtual string Type { get; set; }
        public virtual string SubjectId { get; set; }
        public virtual string ClientId { get; set; }
        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Data { get; set; }

    }
}
