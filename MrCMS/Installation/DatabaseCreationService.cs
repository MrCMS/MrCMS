using System;
using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.DbConfiguration.Helpers;
using MrCMS.Helpers;
using MrCMS.Settings;
using Ninject;

namespace MrCMS.Installation
{
    public class DatabaseCreationService : IDatabaseCreationService
    {
        private readonly IKernel _kernel;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;

        public DatabaseCreationService(IKernel kernel,
            ISystemConfigurationProvider systemConfigurationProvider)
        {
            _kernel = kernel;
            _systemConfigurationProvider = systemConfigurationProvider;
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
                return null;
            createDatabase.CreateDatabase(model);
            SaveConnectionSettings(createDatabase, model);
            return
                _kernel.GetAll<IDatabaseProvider>().FirstOrDefault(provider => provider.Type == model.DatabaseProvider);
        }

        private ICreateDatabase GetDatabaseCreator(InstallModel model)
        {
            Type creatorType =
                TypeHelper.GetAllConcreteTypesAssignableFrom(
                    typeof (ICreateDatabase<>).MakeGenericType(TypeHelper.GetTypeByName(model.DatabaseProvider)))
                    .FirstOrDefault();
            if (creatorType == null)
                return null;
            return _kernel.Get(creatorType) as ICreateDatabase;
        }

        public void SaveConnectionSettings(ICreateDatabase provider, InstallModel installModel)
        {
            var databaseSettings = _systemConfigurationProvider.GetSystemSettings<DatabaseSettings>();

            databaseSettings.ConnectionString = provider.GetConnectionString(installModel);
            databaseSettings.DatabaseProviderType = installModel.DatabaseProvider;

            _systemConfigurationProvider.SaveSettings(databaseSettings);
        }
    }
}