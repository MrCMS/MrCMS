using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class ClientScope : SystemEntity
    {
        public virtual string Scope { get; set; }

        public virtual Client Client { get; set; }
    }
}