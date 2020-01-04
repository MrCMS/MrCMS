using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Installation.Models;
using MrCMS.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace MrCMS.Installation.Services
{
    public class DatabaseCreationService : IDatabaseCreationService
    {
        private const string ConfigKey = "Database";
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _environment;

        public DatabaseCreationService(IServiceProvider serviceProvider, IWebHostEnvironment environment)
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

        public bool IsDatabaseInstalled()
        {
            var environmentName = _environment.EnvironmentName;
            var info = _environment.ContentRootFileProvider.GetFileInfo($"appsettings.{environmentName}.json");
            var config = GetConfig(info);

            if (config.ContainsKey(ConfigKey) && config[ConfigKey] is DatabaseSettings settings)
            {
                return !string.IsNullOrWhiteSpace(settings.ConnectionString);
            }
            return false;
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
            var info = GetEnvironmentAppSettingsInfo();
            var config = GetConfig(info);

            config[ConfigKey] = new DatabaseSettings
            {
                DatabaseProviderType = installModel.DatabaseProvider,
                ConnectionString = provider.GetConnectionString(installModel)
            };

            File.WriteAllText(info.PhysicalPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        private static IDictionary<String, object> GetConfig(IFileInfo info)
        {
            IDictionary<String, object> config;
            if (info.Exists)
            {
                using (var stream = info.CreateReadStream())
                using (TextReader reader = new StreamReader(stream))
                {
                    config = JsonConvert.DeserializeObject<ExpandoObject>(reader.ReadToEnd());
                }
            }
            else
            {
                config = new ExpandoObject();
            }

            return config;
        }

        private IFileInfo GetEnvironmentAppSettingsInfo()
        {
            var environmentName = _environment.EnvironmentName;
            var info = _environment.ContentRootFileProvider.GetFileInfo($"appsettings.{environmentName}.json");

            //path = Path.Combine(_environment.ContentRootPath, $"appsettings.{environmentName}.json");
            return info;
        }
    }
}