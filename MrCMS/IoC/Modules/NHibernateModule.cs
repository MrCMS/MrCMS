using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    public class NHibernateModule : NinjectModule
    {
        private readonly bool _cacheEnabled = true;
        private NHibernateConfigurator _configurator;

        public NHibernateModule(bool cacheEnabled = true)
        {
            _cacheEnabled = cacheEnabled;
        }

        public override void Load()
        {
            Kernel.Rebind<IDatabaseProvider>().ToMethod(context =>
            {
                if (context.Kernel.Get<IEnsureDatabaseIsInstalled>().IsInstalled())
                {
                    var databaseSettings = context.Kernel.Get<DatabaseSettings>();
                    var typeByName = TypeHelper.GetTypeByName(databaseSettings.DatabaseProviderType);
                    if (typeByName == null)
                        return null;
                    return context.Kernel.Get(typeByName) as IDatabaseProvider;
                }
                return null;
            });
            _configurator = _configurator ?? new NHibernateConfigurator(Kernel.Get<IDatabaseProvider>());
            _configurator.CacheEnabled = _cacheEnabled;

            Kernel.Bind<ISessionFactory>()
                .ToMethod(
                    context => _configurator.CreateSessionFactory())
                .InSingletonScope();

            Kernel.Bind<ISession>().ToMethod(
                context =>
                     context.Kernel.Get<ISessionFactory>().OpenFilteredSession()).InRequestScope();

            Kernel.Bind<IStatelessSession>()
                .ToMethod(context => context.Kernel.Get<ISessionFactory>().OpenStatelessSession()).InRequestScope();
        }
    }
}