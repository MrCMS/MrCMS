using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class ApiResourceClaimPOverride : IAutoMappingOverride<ApiResourceClaim>
    {
        public void Override(AutoMapping<ApiResourceClaim> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Map(item => item.Type).Length(250).Not.Nullable();
        }
    }


}
