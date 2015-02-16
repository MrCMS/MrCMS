using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Installation
{
    public class InstallationService : IInstallationService
    {
        private readonly IDatabaseCreationService _databaseCreationService;
        private readonly IFileSystemAccessService _fileSystemAccessService;

        public InstallationService(IFileSystemAccessService fileSystemAccessService, IDatabaseCreationService databaseCreationService)
        {
            _fileSystemAccessService = fileSystemAccessService;
            _databaseCreationService = databaseCreationService;
        }

        public InstallationResult Install(InstallModel model)
        {
            if (model.DatabaseConnectionString != null)
                model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();

            InstallationResult result = _databaseCreationService.ValidateConnectionString(model);
            if (!result.Success)
                return result;

            result = _fileSystemAccessService.EnsureAccessToFileSystem();
            if (!result.Success)
                return result;
            _fileSystemAccessService.EmptyAppData();

            try
            {
                IDatabaseProvider provider = _databaseCreationService.CreateDatabase(model);

                //save settings
                SetUpInitialData(model, provider);

                CurrentRequestData.OnEndRequest.Add(new ApplicationRestart());
            }
            catch (Exception exception)
            {
                result.AddModelError("Setup failed: " + exception);
                _fileSystemAccessService.EmptyAppData();
            }

            return result;
        }

        public List<DatabaseProviderInfo> GetProviderTypes()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<IDatabaseProvider>()
                .Select(type => new DatabaseProviderInfo(type)).ToList();
        }

        private void SetUpInitialData(InstallModel model, IDatabaseProvider provider)
        {
            var configurator = new NHibernateConfigurator(provider);

            ISessionFactory sessionFactory = configurator.CreateSessionFactory();
            ISession session = sessionFactory.OpenFilteredSession();
            IStatelessSession statelessSession = sessionFactory.OpenStatelessSession();
            var kernel = MrCMSApplication.Get<IKernel>();
            kernel.Rebind<ISession>().ToMethod(context => session);
            kernel.Rebind<IStatelessSession>().ToMethod(context => statelessSession);
            var site = new Site
            {
                Name = model.SiteName,
                BaseUrl = model.SiteUrl,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            using (ITransaction transaction = statelessSession.BeginTransaction())
            {
                statelessSession.Insert(site);
                transaction.Commit();
            }
            CurrentRequestData.CurrentSite = site;

            kernel.Get<IInitializeDatabase>().Initialize(model);
            kernel.Get<ICreateInitialUser>().Create(model);
            kernel.GetAll<IOnInstallation>()
                .OrderBy(installation => installation.Priority)
                .ForEach(installation => installation.Install(model));
        }
    }
}