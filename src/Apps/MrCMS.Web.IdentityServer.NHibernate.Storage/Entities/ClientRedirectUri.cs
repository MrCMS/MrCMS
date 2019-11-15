using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class ClientRedirectUri : SystemEntity
    {
        public virtual string RedirectUri { get; set; }

        public virtual Client Client { get; set; }
    }
}