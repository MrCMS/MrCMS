using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Services
{
    public class CustomSiteMapService : ICustomSiteMapService
    {
        private readonly IKernel _kernel;
        public static readonly Dictionary<Type, HashSet<Type>> CustomSiteMapsTypes;

        static CustomSiteMapService()
        {
            CustomSiteMapsTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (
                Type type in
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                Type thisType = type;
                while (typeof(Webpage).IsAssignableFrom(thisType))
                {
                    foreach (Type assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(CustomSiteMapBase<>).MakeGenericType(type)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;
                }

                CustomSiteMapsTypes.Add(type, hashSet);
            }
        }

        public CustomSiteMapService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void AddCustomSiteMapData(Webpage webpage, XmlNode mainNode, XmlDocument document)
        {
            if (webpage == null)
            {
                return;
            }
            var type = webpage.GetType();
            if (CustomSiteMapsTypes.ContainsKey(type))
            {
                foreach (
                    var binderType in
                        CustomSiteMapsTypes[type].Select(assignViewDataType => _kernel.Get(assignViewDataType))
                    )
                {
                    var customBinder = binderType as CustomSiteMapBase;
                    if (customBinder != null) customBinder.AddCustomSiteMapData(webpage, mainNode, document);
                }
            }
        }
    }
}