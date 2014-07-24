using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class SetAdminViewData : ISetAdminViewData
    {
        private readonly IKernel _kernel;

        static SetAdminViewData()
        {
            AssignViewDataTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>().Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                var thisType = type;
                while (typeof(Webpage).IsAssignableFrom(thisType))
                {
                    foreach (var assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(BaseAssignAdminViewData<>).MakeGenericType(type)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;

                }

                AssignViewDataTypes.Add(type, hashSet);
            }
        }

        public SetAdminViewData(IKernel kernel)
        {
            _kernel = kernel;
        }

        private static readonly Dictionary<Type, HashSet<Type>> AssignViewDataTypes;

        public void SetViewData<T>(T webpage, ViewDataDictionary viewData) where T : Webpage
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
                        AssignViewDataTypes[type].Select(assignViewDataType => _kernel.Get(assignViewDataType))
                    )
                {
                    var adminViewData = assignAdminViewData as BaseAssignAdminViewData;
                    if (adminViewData != null) adminViewData.AssignViewData(webpage, viewData);
                }
            }
        }
    }
}