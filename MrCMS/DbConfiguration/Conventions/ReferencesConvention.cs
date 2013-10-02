using System.Collections.Generic;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MrCMS.DbConfiguration.Conventions
{
    public class ReferencesConvention : IReferenceConvention
    {
        private static readonly List<string> Indices = new List<string>();
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
            instance.NotFound.Ignore();

            instance.ForeignKey(string.Format("FK_{0}_{1}",
                (instance.EntityType != null) ? instance.EntityType.Name : instance.Name, instance.Property.Name));


            var index = string.Format("IX_{0}_{1}", instance.EntityType.Name, instance.Property.Name);
            if (Indices.Contains(index)) return;
            instance.Index(index);
            Indices.Add(index);
        
        }
    }
}