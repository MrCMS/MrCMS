using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Services
{
    public class SetWidgetAdminViewData : ISetWidgetAdminViewData
    {
        private readonly IServiceProvider _serviceProvider;

        static SetWidgetAdminViewData()
        {
            AssignViewDataTypes = new Dictionary<Type, HashSet<Type>>();
            var allTypes = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(BaseAssignWidgetAdminViewData<>));

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>()
                .Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                var thisType = type;
                while (typeof(Widget).IsAssignableFrom(thisType))
                {
                    foreach (var assignType in allTypes.FindAll(x =>
                        typeof(BaseAssignWidgetAdminViewData<>).MakeGenericType(thisType).IsAssignableFrom(x)))
                    {
                        hashSet.Add(assignType);
                    }

                    thisType = thisType.BaseType;
                }

                AssignViewDataTypes.Add(type, hashSet);
            }
        }

        public SetWidgetAdminViewData(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static readonly Dictionary<Type, HashSet<Type>> AssignViewDataTypes;

        public async Task SetViewData<T>(ViewDataDictionary viewData, T widget) where T : Widget
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
                    AssignViewDataTypes[type]
                        .Select(assignViewDataType => _serviceProvider.GetService(assignViewDataType))
                )
                {
                    var adminViewData = assignAdminViewData as BaseAssignWidgetAdminViewData;
                    if (adminViewData != null) await adminViewData.AssignViewDataBase(widget, viewData);
                }
            }
        }

        public async Task SetViewDataForAdd(ViewDataDictionary viewData, string type)
        {
            var widgetType = TypeHelper.GetTypeByName(type);
            if (widgetType.IsAbstract || !typeof(Widget).IsAssignableFrom(widgetType))
                return;

            await SetViewData(viewData, Activator.CreateInstance(widgetType) as Widget);
        }
    }
}