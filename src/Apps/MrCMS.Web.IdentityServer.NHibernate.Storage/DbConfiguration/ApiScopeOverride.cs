using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class ApiScopeOverride : IAutoMappingOverride<ApiScope>
    {
        public void Override(AutoMapping<ApiScope> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Table(storeOptions.ApiScope.Name);
        }
    }


}
