using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class SetWidgetAdminViewData : ISetWidgetAdminViewData
    {
        private readonly IKernel _kernel;

        static SetWidgetAdminViewData()
        {
            AssignViewDataTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>().Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                var thisType = type;
                while (typeof(Widget).IsAssignableFrom(thisType))
                {
                    foreach (var assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(BaseAssignWidgetAdminViewData<>).MakeGenericType(thisType)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;

                }

                AssignViewDataTypes.Add(type, hashSet);
            }
        }

        public SetWidgetAdminViewData(IKernel kernel)
        {
            _kernel = kernel;
        }

        private static readonly Dictionary<Type, HashSet<Type>> AssignViewDataTypes;

        public void SetViewData<T>(T widget, ViewDataDictionary viewData) where T : Widget
        {
            if (widget == null)
            {
                return;
            }
            var type = widget.GetType();
            if (AssignViewDataTypes.ContainsKey(type))
            {
                foreach (
                    var assignAdminViewData in
                        AssignViewDataTypes[type].Select(assignViewDataType => _kernel.Get(assignViewDataType))
                    )
                {
                    var adminViewData = assignAdminViewData as BaseAssignWidgetAdminViewData;
                    if (adminViewData != null) adminViewData.AssignViewDataBase(widget, viewData);
                }
            }
        }
    }
}