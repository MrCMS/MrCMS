using MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class ClientProperty : Property
    {
        public virtual Client Client { get; set; }
    }
}