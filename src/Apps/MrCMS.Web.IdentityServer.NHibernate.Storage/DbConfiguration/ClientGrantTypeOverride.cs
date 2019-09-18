using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class ClientGrantTypeOverride : IAutoMappingOverride<ClientGrantType>
    {
        public void Override(AutoMapping<ClientGrantType> mapping)
        {

            ConfigurationStoreOptions storeOptions = new ConfigurationStoreOptions();
            mapping.Table(storeOptions.ClientGrantType.Name);
        }
    }


}
