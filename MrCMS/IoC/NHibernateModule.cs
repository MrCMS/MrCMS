using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Helpers;
using NHibernate;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    public class NHibernateModule : NinjectModule
    {
        public NHibernateModule(DatabaseType databaseType, bool inDevelopment = false,
                                bool forWebsite = true, bool cacheEnabled = true)
        {
            _configurator = new NHibernateConfigurator { CacheEnabled = cacheEnabled, DatabaseType = databaseType, InDevelopment = inDevelopment };
            _forWebsite = forWebsite;
        }

        private readonly NHibernateConfigurator _configurator;

        private readonly bool _forWebsite;

        public override void Load()
        {
            Kernel.Bind<ISessionFactory>().ToMethod(context => _configurator.CreateSessionFactory()).InSingletonScope();
            
            if (_forWebsite)
            {
                Kernel.Bind<ISession>().ToMethod(
                    context =>
                    context.Kernel.Get<ISessionFactory>().OpenSession()).InRequestScope();
            }   
            else
            {
                Kernel.Bind<ISession>().ToMethod(context => context.Kernel.Get<ISessionFactory>().OpenSession()).
                    InThreadScope();
            }
        }
    }
}