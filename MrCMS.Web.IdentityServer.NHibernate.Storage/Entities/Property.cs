using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public abstract class Property : SystemEntity
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}