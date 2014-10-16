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
        private readonly bool _forWebsite;
        private NHibernateConfigurator _configurator;

        public NHibernateModule(bool forWebsite = true, bool cacheEnabled = true)
        {
            _cacheEnabled = cacheEnabled;
            _forWebsite = forWebsite;
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
            _configurator = _configurator ?? new NHibernateConfigurator(Kernel.Get<IDatabaseProvider>(),Kernel.Get<ISystemConfigurationProvider>());
            _configurator.CacheEnabled = _cacheEnabled;

            Kernel.Bind<ISessionFactory>()
                .ToMethod(
                    context => _configurator.CreateSessionFactory())
                .InSingletonScope();

            if (_forWebsite)
            {
                Kernel.Bind<ISession>().ToMethod(
                    context =>
                         context.Kernel.Get<ISessionFactory>().OpenFilteredSession()).InRequestScope();
            }
            else
            {
                Kernel.Bind<ISession>()
                    .ToMethod(
                        context => context.Kernel.Get<ISessionFactory>().OpenFilteredSession())
                    .InThreadScope();
            }
        }
    }
}