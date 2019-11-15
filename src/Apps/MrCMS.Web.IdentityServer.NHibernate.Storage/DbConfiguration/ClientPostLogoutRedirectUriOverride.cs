using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class ClientPostLogoutRedirectUriOverride : IAutoMappingOverride<ClientPostLogoutRedirectUri>
    {
        public void Override(AutoMapping<ClientPostLogoutRedirectUri> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Table(storeOptions.ClientPostLogoutRedirectUri.Name);
        }
    }


}
