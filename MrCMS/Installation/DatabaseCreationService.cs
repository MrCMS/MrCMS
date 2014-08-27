using System;
using System.Linq;
using MrCMS.DbConfiguration;
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

        public IDatabaseProvider CreateDatabase(InstallModel model)
        {
            Type creatorType =
                TypeHelper.GetAllConcreteTypesAssignableFrom(
                    typeof (ICreateDatabase<>).MakeGenericType(TypeHelper.GetTypeByName(model.DatabaseProvider)))
                    .FirstOrDefault();
            if (creatorType == null)
                return null;
            var createDatabase = _kernel.Get(creatorType) as ICreateDatabase;
            createDatabase.CreateDatabase(model);
            SaveConnectionSettings(createDatabase, model);
            return
                _kernel.GetAll<IDatabaseProvider>().FirstOrDefault(provider => provider.Type == model.DatabaseProvider);
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