using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Installation.Models;
using MrCMS.Website;

namespace MrCMS.Installation.Services
{
    public class InstallationService : IInstallationService
    {
        //private readonly IDatabaseCreationService _databaseCreationService;
        //private readonly IServiceCollection _serviceCollection;
        private readonly IFileSystemAccessService _fileSystemAccessService;
        private readonly IServiceProvider _serviceProvider;

        public InstallationService(IFileSystemAccessService fileSystemAccessService, IServiceProvider serviceProvider)//, IDatabaseCreationService databaseCreationService, IServiceCollection serviceCollection)
        {
            _fileSystemAccessService = fileSystemAccessService;
            _serviceProvider = serviceProvider;
            //_databaseCreationService = databaseCreationService;
            //_serviceCollection = serviceCollection;
        }

        public async Task<InstallationResult> Install(InstallModel model)
        {
            //if (model.DatabaseConnectionString != null)
            //{
            //    model.DatabaseConnectionString = model.DatabaseConnectionString.Trim();
            //}

            //InstallationResult result = _databaseCreationService.ValidateConnectionString(model);
            //if (!result.Success)
            //{
            //    return result;
            //}

            var result = _fileSystemAccessService.EnsureAccessToFileSystem();
            if (!result.Success)
            {
                return result;
            }

            _fileSystemAccessService.EmptyAppData();

            try
            {
                //IDatabaseProvider provider = _databaseCreationService.CreateDatabase(model);

                //save settings
                await SetUpInitialData(model);
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

        public bool DatabaseIsInstalled()
        {
            try
            {
                return _serviceProvider.GetRequiredService<IGlobalRepository<Site>>().Readonly().Any();
            }
            catch
            {
                return false;
            }
        }

        private async Task SetUpInitialData(InstallModel model)
        {
            var site = new Site
            {
                Name = model.SiteName,
                BaseUrl = model.SiteUrl,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {


                    //_serviceCollection.AddDbContext<WebsiteContext>(provider.SetupAction);
                    //var serviceProvider = _serviceCollection.BuildServiceProvider();

                    //ISessionFactory sessionFactory = configurator.CreateSessionFactory();
                    //ISession session = sessionFactory.OpenFilteredSession(serviceProvider);
                    //IStatelessSession statelessSession = sessionFactory.OpenStatelessSession();
                    //context.Items["override-nh-session"] = session;
                    //context.Items["override-nh-stateless-session"] = statelessSession;

                    var repository = serviceProvider.GetRequiredService<IGlobalRepository<Site>>();

                    var setSiteId = serviceProvider.GetRequiredService<ISetSiteId>();

                    await repository.Add(site);
                    var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    var context = contextAccessor.HttpContext;
                    setSiteId.SetId(context);


                    await serviceProvider.GetRequiredService<IInitializeDatabase>().Initialize(model);
                    await serviceProvider.GetRequiredService<ICreateInitialUser>().Create(model);
                    var onInstallations = serviceProvider.GetServices<IOnInstallation>()
                        .OrderBy(installation => installation.Priority).ToList();
                    foreach (var onInstallation in onInstallations)
                    {
                        await onInstallation.Install(model);
                    }
                }
                catch
                {
                    var databaseFacade = serviceProvider.GetService<ISystemDatabase>().Database;
                    databaseFacade.EnsureDeleted();
                    databaseFacade.Migrate();

                    throw;
                }
            }
        }
    }
}