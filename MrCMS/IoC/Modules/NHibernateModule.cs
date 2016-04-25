using System.Web;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    public class NHibernateModule : NinjectModule
    {
        internal const string ModuleName = "MrCMS-NHibernate-Module";

        public override string Name
        {
            get { return ModuleName; }
        }

        private readonly bool _cacheEnabled;
        private readonly bool _forWebsite;

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

            Kernel.Bind<ISessionFactory>()
                .ToMethod(
                    context =>
                    {
                        var configurator = new NHibernateConfigurator(context.Kernel.Get<IDatabaseProvider>())
                        {
                            CacheEnabled = _cacheEnabled
                        };
                        return configurator.CreateSessionFactory();
                    })
                .InSingletonScope();

            if (_forWebsite)
            {
                Kernel.Bind<ISession>().ToMethod(GetSession).InRequestScope();
            }
            else
            {
                Kernel.Bind<ISession>().ToMethod(GetSession).InThreadScope();
            }
            Kernel.Bind<IStatelessSession>()
                .ToMethod(context => context.Kernel.Get<ISessionFactory>().OpenStatelessSession()).InRequestScope();
        }

        public override void Unload()
        {
            Kernel.Unbind<IDatabaseProvider>();
            Kernel.Unbind<ISessionFactory>();
            Kernel.Unbind<ISession>();
            Kernel.Unbind<IStatelessSession>();
        }

        private static ISession GetSession(IContext context)
        {
            HttpContextBase httpContext = null;
            try
            {
                httpContext = context.Kernel.Get<HttpContextBase>();
            }
            catch
            {
            }
            return context.Kernel.Get<ISessionFactory>().OpenFilteredSession(httpContext);
        }
    }
}