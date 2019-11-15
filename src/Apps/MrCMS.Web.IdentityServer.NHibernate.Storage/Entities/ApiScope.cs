using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{

    public class ApiScope : SystemEntity
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Required { get; set; }
        public virtual bool Emphasize { get; set; }
        public virtual bool ShowInDiscoveryDocument { get; set; } = true;
        public virtual ISet<ApiScopeClaim> UserClaims { get; set; } = new HashSet<ApiScopeClaim>();
        public virtual ApiResource ApiResource { get; set; }
    }
}