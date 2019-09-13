using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class IdentityResourcePropertyOverride : IAutoMappingOverride<IdentityResourceProperty>
    {
        public void Override(AutoMapping<IdentityResourceProperty> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Table(storeOptions.IdentityResourceProperty.Name);
        }
    }


}
