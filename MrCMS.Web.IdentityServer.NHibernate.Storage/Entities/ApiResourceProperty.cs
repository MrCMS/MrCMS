namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class ApiResourceProperty : Property
    {
        public virtual ApiResource ApiResource { get; set; }
    }
}