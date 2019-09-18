namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class IdentityClaim : UserClaim
    {
        public virtual IdentityResource IdentityResource { get; set; }
    }
}
