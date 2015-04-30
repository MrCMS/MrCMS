using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Services.CloneSite
{
    public class CloneWebpageSiteParts : ICloneWebpageSiteParts
    {
        private static readonly Dictionary<Type, HashSet<Type>> CloneWebpagePartTypes;
        private readonly IKernel _kernel;

        static CloneWebpageSiteParts()
        {
            CloneWebpagePartTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (Type type in
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                Type thisType = type;
                while (thisType != null && typeof (Webpage).IsAssignableFrom(thisType))
                {
                    foreach (Type assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof (CloneWebpagePart<>).MakeGenericType(thisType)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;
                }

                CloneWebpagePartTypes.Add(type, hashSet);
            }
        }

        public CloneWebpageSiteParts(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Clone(Webpage @from, Webpage to, SiteCloneContext siteCloneContext)
        {
            if (from == null || to == null)
            {
                return;
            }
            Type type = from.GetType();
            if (CloneWebpagePartTypes.ContainsKey(type))
            {
                foreach (
                    object cloneWebpagePart in
                        CloneWebpagePartTypes[type].Select(service => _kernel.Get(service))
                    )
                {
                    var part = cloneWebpagePart as CloneWebpagePart;
                    if (part != null) part.ClonePartBase(from, to, siteCloneContext);
                }
            }
        }
    }
}