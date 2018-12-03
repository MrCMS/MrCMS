using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Settings;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace MrCMS.Installation.Services
{
    public class DatabaseCreationService : IDatabaseCreationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _environment;

        public DatabaseCreationService(IServiceProvider serviceProvider, IHostingEnvironment environment)
        {
            _serviceProvider = serviceProvider;
            _environment = environment;
        }

        public InstallationResult ValidateConnectionString(InstallModel model)
        {
            var result = new InstallationResult();
            ICreateDatabase createDatabase = GetDatabaseCreator(model);
            if (createDatabase == null)
            {
                result.AddModelError("Cannot validate connection string for model.");
                return result;
            }
            if (!createDatabase.ValidateConnectionString(model))
            {
                result.AddModelError("Unable to create the connection string with the provided details.");
            }
            return result;
        }

        public IDatabaseProvider CreateDatabase(InstallModel model)
        {
            ICreateDatabase createDatabase = GetDatabaseCreator(model);
            if (createDatabase == null)
            {
                return null;
            }

            var provider = createDatabase.CreateDatabase(model);
            SaveConnectionSettings(createDatabase, model);
            return provider;
        }

        private ICreateDatabase GetDatabaseCreator(InstallModel model)
        {
            Type creatorType =
                TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(ICreateDatabase<>).MakeGenericType(TypeHelper.GetTypeByName(model.DatabaseProvider)))
                    .FirstOrDefault();
            if (creatorType == null)
            {
                return null;
            }

            return _serviceProvider.GetRequiredService(creatorType) as ICreateDatabase;
        }

        public void SaveConnectionSettings(ICreateDatabase provider, InstallModel installModel)
        {
            var environmentName = _environment.EnvironmentName;
            var info = _environment.ContentRootFileProvider.GetFileInfo($"appsettings.{environmentName}.json");

            var path = Path.Combine(_environment.ContentRootPath, $"appsettings.{environmentName}.json");
            dynamic config;
            if (info.Exists)
            {
                using (var stream = info.CreateReadStream())
                using (TextReader reader = new StreamReader(stream))
                    config = JsonConvert.DeserializeObject<ExpandoObject>(reader.ReadToEnd());
            }
            else
            {
                config = new ExpandoObject();
            }
            //File.Exists(path)
            //? JsonConvert.DeserializeObject<ExpandoObject>(File.ReadAllText(path))
            //: new ExpandoObject();

            config.Database = new DatabaseSettings
            {
                DatabaseProviderType = installModel.DatabaseProvider,
                ConnectionString = provider.GetConnectionString(installModel)
            };

            File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));

            //var databaseSettings = _systemConfigurationProvider.GetSystemSettings<DatabaseSettings>();

            //databaseSettings.ConnectionString = provider.GetConnectionString(installModel);
            //databaseSettings.DatabaseProviderType = installModel.DatabaseProvider;

            //_systemConfigurationProvider.SaveSettings(databaseSettings);
        }
    }
}