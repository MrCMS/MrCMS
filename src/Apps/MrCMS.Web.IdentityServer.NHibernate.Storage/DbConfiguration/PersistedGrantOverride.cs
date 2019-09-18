using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Options;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.DbConfiguration
{

    public class PersistedGrantOverride : IAutoMappingOverride<PersistedGrant>
    {
        public void Override(AutoMapping<PersistedGrant> mapping)
        {
            OperationalStoreOptions storeOptions = new OperationalStoreOptions();
            mapping.Table(storeOptions.PersistedGrants.Name);
            mapping.Map(x => x.Key).Length(200);
            mapping.Map(x => x.SubjectId).Index("IX_Subject_Subject_Id");
            mapping.Map(x => x.ClientId).Index("IX_Subject_Client_Id");
            mapping.Map(x => x.Type).Index("IX_Subject_Client_Type");
        }
    }


}
