using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class ClientClaim : SystemEntity
    {
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }

        public virtual Client Client { get; set; }
    }
}