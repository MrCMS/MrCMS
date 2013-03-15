using System;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Automapping;
using MrCMS.Apps;
using MrCMS.Helpers;

namespace MrCMS.DbConfiguration
{
    public static class ConfigHelper
    {
        public static AutoPersistenceModel UseOverridesFromAssemblies(this AutoPersistenceModel model, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies.Where(assembly => !assembly.IsDynamic && !assembly.GlobalAssemblyCache))
            {
                model.UseOverridesFromAssembly(assembly);
            }
            return model;
        }

        public static AutoPersistenceModel IncludeAppBases(this AutoPersistenceModel model)
        {
            foreach (var baseType in TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSApp>()
                                        .Select(type => Activator.CreateInstance(type) as MrCMSApp)
                                        .SelectMany(app => app.BaseTypes))
            {
                model.IncludeBase(baseType);
            }
            return model;
        }
    }
}