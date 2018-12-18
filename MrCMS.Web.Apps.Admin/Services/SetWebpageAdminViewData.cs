using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class SetWebpageAdminViewData : ISetWebpageAdminViewData
    {
        private readonly IServiceProvider _serviceProvider;

        static SetWebpageAdminViewData()
        {
            AssignViewDataTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>().Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                var thisType = type;
                while (thisType != null && typeof(Webpage).IsAssignableFrom(thisType))
                {
                    foreach (var assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(BaseAssignWebpageAdminViewData<>).MakeGenericType(thisType)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;

                }

                AssignViewDataTypes.Add(type, hashSet);
            }
        }

        public SetWebpageAdminViewData(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static readonly Dictionary<Type, HashSet<Type>> AssignViewDataTypes;

        public void SetViewData<T>(ViewDataDictionary viewData, T webpage) where T : Webpage
        {
            if (webpage == null)
            {
                return;
            }
            var type = webpage.GetType();
            if (AssignViewDataTypes.ContainsKey(type))
            {
                foreach (
                    var assignAdminViewData in
                        AssignViewDataTypes[type].Select(assignViewDataType => _serviceProvider.GetService(assignViewDataType))
                    )
                {
                    var adminViewData = assignAdminViewData as BaseAssignWebpageAdminViewData;
                    if (adminViewData != null) adminViewData.AssignViewDataBase(webpage, viewData);
                }
            }
        }
    }
}