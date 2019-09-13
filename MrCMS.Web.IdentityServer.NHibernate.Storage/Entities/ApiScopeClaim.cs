using UserClaim = MrCMS.Web.IdentityServer.NHibernate.Storage.Entities.UserClaim;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{


    public class ApiScopeClaim : UserClaim
    {
        public virtual ApiScope ApiScope { get; set; }
    }
}