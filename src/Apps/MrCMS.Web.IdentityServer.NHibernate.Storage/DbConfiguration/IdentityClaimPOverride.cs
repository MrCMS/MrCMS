using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class IdentityClaimPOverride : IAutoMappingOverride<IdentityClaim>
    {
        public void Override(AutoMapping<IdentityClaim> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Table(storeOptions.IdentityClaim.Name);
            mapping.Map(item => item.Type).Length(250).Not.Nullable();
        }
    }


}
