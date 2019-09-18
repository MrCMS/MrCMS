using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    public class ApiResource : SystemEntity
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }

        public virtual ISet<ApiSecret> Secrets { get; set; } = new HashSet<ApiSecret>();
        public virtual ISet<ApiScope> Scopes { get; set; } = new HashSet<ApiScope>();
        public virtual ISet<ApiResourceClaim> UserClaims { get; set; } = new HashSet<ApiResourceClaim>();
        public virtual ISet<ApiResourceProperty> Properties { get; set; } = new HashSet<ApiResourceProperty>();

        public virtual DateTime Created { get; set; } = DateTime.UtcNow;
        public virtual DateTime? Updated { get; set; }
        public virtual DateTime? LastAccessed { get; set; }
        public virtual bool NonEditable { get; set; }
    }

    
}
