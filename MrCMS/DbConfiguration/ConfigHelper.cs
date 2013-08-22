using System;
using System.Linq;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;
using MrCMS.Apps;
using MrCMS.DbConfiguration.Types;
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

        public static AutoPersistenceModel IncludeAppConventions(this AutoPersistenceModel model)
        {
            foreach (var baseType in TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSApp>()
                                        .Select(type => Activator.CreateInstance(type) as MrCMSApp)
                                        .SelectMany(app => app.Conventions))
            {
                model.Conventions.Add(baseType);
            }
            return model;
        }

        /// <summary>
        /// Shortcut to make a property varchar(max) as you have to explicitly set the length otherwise
        /// </summary>
        /// <param name="propertyPart"></param>
        /// <returns></returns>
        public static PropertyPart MakeVarCharMax(this PropertyPart propertyPart)
        {
            return propertyPart.CustomType<VarcharMax>().Length(4001);
        }
    }
}