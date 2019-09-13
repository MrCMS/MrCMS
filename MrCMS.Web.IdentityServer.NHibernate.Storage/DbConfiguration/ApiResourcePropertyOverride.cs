using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class ApiResourcePropertyOverride : IAutoMappingOverride<ApiResourceProperty>
    {
        public void Override(AutoMapping<ApiResourceProperty> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
           mapping.Table(storeOptions.ApiResourceProperty.Name);
           mapping.Map(item => item.Value).Length(2500).Not.Nullable();
        }
    }


}
