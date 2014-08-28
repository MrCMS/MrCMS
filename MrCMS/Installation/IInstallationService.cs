using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;

namespace MrCMS.Installation
{
    public interface IInstallationService
    {
        InstallationResult Install(InstallModel model);
        List<DatabaseProviderInfo> GetProviderTypes();
    }

    public class DatabaseProviderInfo
    {
        public DatabaseProviderInfo(Type type)
        {
            Description = type.GetDescription();
            RequiresConnectionStringBuilding =
                type.GetCustomAttribute<NoConnectionStringBuilderAttribute>() == null;
            TypeName = type.FullName;
        }

        public string TypeName { get; set; }
        public string Description{ get; set; }
        public bool RequiresConnectionStringBuilding { get; set; }
    }
}
