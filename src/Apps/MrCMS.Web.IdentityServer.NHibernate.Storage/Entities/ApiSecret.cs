using MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    public class ApiSecret : Secret
    {
        public virtual ApiResource ApiResource { get; set; }
    }
}