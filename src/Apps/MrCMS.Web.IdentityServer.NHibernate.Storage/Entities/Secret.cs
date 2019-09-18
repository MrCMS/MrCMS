using System;
using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    public abstract class Secret : SystemEntity
    {
        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual string Type { get; set; } = "SharedSecret";
    }
}