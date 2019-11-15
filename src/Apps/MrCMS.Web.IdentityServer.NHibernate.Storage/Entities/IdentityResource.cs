using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class IdentityResource : SystemEntity
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ISet<IdentityClaim> UserClaims { get; set; } = new HashSet<IdentityClaim>();
        public virtual ISet<IdentityResourceProperty> Properties { get; set; } = new HashSet<IdentityResourceProperty>();
        public virtual bool NonEditable { get; set; }

        public virtual DateTime Created { get; set; } = DateTime.UtcNow;
        public virtual DateTime? Updated { get; set; }
    }
}