using System.Linq;
using System.Reflection;
using FluentNHibernate.Automapping;

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
    }
}