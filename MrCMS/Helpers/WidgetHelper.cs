using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Entities.Widget;

namespace MrCMS.Helpers
{
    public static class WidgetHelper
    {
        private static IEnumerable<Type> _widgetTypes;

        public static IEnumerable<Type> WidgetTypes
        {
            get { return _widgetTypes ?? (_widgetTypes = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>()); }
        }

        public static List<SelectListItem> WidgetTypeDropdownItems
        {
            get
            {
                return WidgetTypes.BuildSelectItemList(type => type.Name.BreakUpString(), type => type.Name,
                                                       emptyItemText: null);
            }
        }

        public static Widget GetNewWidget(string widgetType)
        {
            var type = GetTypeByName(widgetType);
            return Activator.CreateInstance(Type.GetType(type.AssemblyQualifiedName)) as Widget;
        }

        public static Widget SetValues(this Widget widget, NameValueCollection collection)
        {
            if (widget == null) return null;

            var propertyInfos = widget.GetType().GetProperties();

            foreach (var propertyInfo in propertyInfos.Where(x => x.DeclaringType != typeof(SiteEntity) && x.CanWrite))
            {
                var value = collection[propertyInfo.Name];

                if (value != null)
                {
                    propertyInfo.SetValue(widget, value, null);
                }
            }

            return widget;
        }

        public static Type GetTypeByName(string typeName)
        {
            return WidgetTypes.FirstOrDefault(x => x.Name == typeName);
        }

        public static Widget CopyNew(this Widget widget)
        {
            var type = widget.GetType();
            var newWidget = Activator.CreateInstance(type) as Widget;

            var propertyInfos = type.GetProperties().Where(info => info.Name != "Id" && info.CanWrite);

            propertyInfos.ForEach(info => info.SetValue(newWidget, info.GetValue(widget, null), null));

            return newWidget;
        }
    }
}