using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class ClientCorsOrigin : SystemEntity
    {
        public virtual string Origin { get; set; }
        public virtual Client Client { get; set; }
    }
}