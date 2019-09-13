using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{
    public class DeviceFlowOverride : IAutoMappingOverride<DeviceFlowCodes>
    {
        public void Override(AutoMapping<DeviceFlowCodes> mapping)
        {
            OperationalStoreOptions storeOptions = new OperationalStoreOptions();
           mapping.Table(storeOptions.DeviceFlowCodes.Name);
            mapping.Map(item => item.UserCode).Generated.Always();
            mapping.Map(item => item.Data).MakeVarCharMax();
        }
    }


}
