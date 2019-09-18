using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class IdentityResourceOverride : IAutoMappingOverride<IdentityResource>
    {
        public void Override(AutoMapping<IdentityResource> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Table(storeOptions.IdentityResource.Name);
            mapping.Map(item => item.Name).Length(250);
        }
    }


}
