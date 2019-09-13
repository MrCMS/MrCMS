using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Conventions;
using NHibernate.Cfg;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage
{
    
    public class MrCMSIs4App : StandardMrCMSApp
    {
        public MrCMSIs4App()
        {
            ContentPrefix = "/Apps/IS4";
            ViewPrefix = "/Apps/IS4";
        }
        public override string Name => "IS4";
        public override string Version => "1.0";

        //public override IEnumerable<Type> Conventions
        //{
        //    get
        //    {
        //        yield return typeof(PersistantGrantPrimaryKeyConvention);
        //    }
        //}

        public override void AppendConfiguration(Configuration configuration)
        {
          //  configuration.
        }

        //public override IServiceCollection RegisterServices(IServiceCollection serviceCollection)
        //{
        //   // serviceCollection.RegisterSwagger();
        //    return base.RegisterServices(serviceCollection);
        //}



        //public override IDictionary<Type, string> SignalRHubs { get; } = new Dictionary<Type, string>
        //{
        //    [typeof(BatchProcessingHub)] = "/batchHub",
        //    [typeof(NotificationHub)] = "/notificationsHub",
        //};
    }
}
