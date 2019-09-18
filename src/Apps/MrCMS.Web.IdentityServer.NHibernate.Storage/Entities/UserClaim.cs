using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    public abstract class UserClaim : SystemEntity
    {
        public virtual string Type { get; set; }
    }
}
