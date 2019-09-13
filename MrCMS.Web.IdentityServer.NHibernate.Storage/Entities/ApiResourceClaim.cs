namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
#pragma warning disable 1591

    public class ApiResourceClaim : UserClaim
    {
        public virtual ApiResource ApiResource { get; set; }
    }
}