using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Impl;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Web.Common;

namespace MrCMS.IoC
{
    public class NHibernateModule : NinjectModule
    {
        public NHibernateModule(NHibernateConfigurator configurator, bool forWebsite = true, Func<ISessionFactory> getSessionFactory = null, Func<ISession> getSession = null)
        {
            _configurator = configurator;
            _forWebsite = forWebsite;
            _getSessionFactory = getSessionFactory;
            _getSession = getSession;
        }
        public NHibernateModule(DatabaseType databaseType, bool inDevelopment = false,
                                bool forWebsite = true, bool cacheEnabled = true)
        {
            _configurator = new NHibernateConfigurator { CacheEnabled = cacheEnabled, DatabaseType = databaseType, InDevelopment = inDevelopment };
            _forWebsite = forWebsite;
        }

        private readonly NHibernateConfigurator _configurator;

        private readonly bool _forWebsite;
        private readonly Func<ISessionFactory> _getSessionFactory;
        private readonly Func<ISession> _getSession;

        public override void Load()
        {
            Kernel.Bind<ISessionFactory>().ToMethod(context => _getSessionFactory != null ? _getSessionFactory() : _configurator.CreateSessionFactory()).InSingletonScope();

            if (_forWebsite)
            {
                Kernel.Bind<ISession>().ToMethod(
                    context => _getSession != null ? _getSession() : context.Kernel.Get<ISessionFactory>().OpenFilteredSession()).InRequestScope();
            }
            else
            {
                Kernel.Bind<ISession>().ToMethod(context => _getSession != null ? _getSession() : context.Kernel.Get<ISessionFactory>().OpenFilteredSession()).
                    InThreadScope();
            }
        }
    }
}