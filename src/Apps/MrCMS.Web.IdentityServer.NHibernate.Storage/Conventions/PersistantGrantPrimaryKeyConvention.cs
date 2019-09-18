using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Conventions
{
    public class PersistantGrantPrimaryKeyConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            Type type = instance.EntityType;
            var persitentType = typeof(PersistedGrant);
            var isMappableModel = persitentType.IsAssignableFrom(type);
            if (isMappableModel)
            {
                // instance.Column("Charles");
                instance.Column("[Key]");
                instance.GeneratedBy.Assigned();
                instance.CustomType("string");
                instance.Length(200);
            }
          
        }
    }
}
